using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Plugins.Core.PathCore;
using Dreamteck.Splines;
using MainGame;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.Event;
using MikroFramework.Pool;
using MikroFramework.TimeSystem;
using UnityEngine;

public class LevelObject : AbstractMikroController<CardGame> {
    [SerializeField]
    private LevelType levelType;
    public LevelType LevelType {
        get => levelType;
        set => levelType = value;
    }

    [SerializeField]
    public GraphVertex Node;

    [SerializeField] private GameObject movingEnemySprite;
    private SafeGameObjectPool movingEnemyPool;

    private Animator animator;

    public bool Interactable = false;

    [SerializeField] private Sprite[] levelTypeSirSprites;
    [SerializeField] private List<GameObject> obstaclePrefabs;

    public Dictionary<MapNode, Vector3> Neighbours2ConnectionLineMidPoints = new Dictionary<MapNode, Vector3>();
    private Dictionary<MapNode, GameObject> Connection2Obstacle = new Dictionary<MapNode, GameObject>();
   
    private void Awake()
    {
         animator = GetComponent<Animator>();
         movingEnemyPool = GameObjectPoolManager.Singleton.GetOrCreatePool(movingEnemySprite);

      
         //Node.Value.LevelType.RegisterWithInitValue(OnNodeTypeChange).UnRegisterWhenGameObjectDestroyed(gameObject);
        

    }

    private void OnDisable() {
        if (Neighbours2ConnectionLineMidPoints != null) {
            Neighbours2ConnectionLineMidPoints.Clear();
            Connection2Obstacle.Clear();
        }
       
    }

    private void Start() {
        this.GetModel<IMapStateModel>().CurNode.RegisterWithInitValue(OnALevelSelected)
            .UnRegisterWhenGameObjectDestroyed(gameObject);
        UpdateNodeSprite();
        this.RegisterEvent<OnEnemyNodeMoveToNewVertex>(OnEnemyNodeMoveToNewVertex)
            .UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<OnMapEnemyKilled>(OnEnemyKilled).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<OnTemporaryBlockedUnDirectedEdgeAdded>(OnRoadBlocked)
       .UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<OnTemporaryBlockedUnDirectedEdgeRecovered>(OnRoadRecovered)
            .UnRegisterWhenGameObjectDestroyed(gameObject);
       Invoke(nameof(UpdateObstacleObj), 0.2f);
    }


    private void UpdateObstacleObj() {
        Graph pathGraph= this.GetSystem<IGameMapGenerationSystem>().GetPathGraph();
        Node.TemporaryBrokenConnections?.ForEach(node => {
            if (node.Depth >= 0) {
                //  Debug.Log(Neighbours2ConnectionLineMidPoints.Count);
               // Debug.Log(node.PointOnMap);
                Vector3 spawnPoint = Neighbours2ConnectionLineMidPoints[pathGraph.FindVertexByValue(node).Value];
                spawnPoint = new Vector3(spawnPoint.x, spawnPoint.y, 0);



                if (!Physics.OverlapSphere(spawnPoint, 1, LayerMask.NameToLayer("Obstacles")).Any()) {
                    GameObject obstacle = SpawnRandomObstacle(spawnPoint);

                    Connection2Obstacle.Add(node, obstacle);
                }
            }
        });
    }

    private GameObject SpawnRandomObstacle(Vector3 spawnPoint) {
        GameObject obstacle = Instantiate(
            obstaclePrefabs[this.GetSystem<ISeedSystem>().GameRandom.Next(0, obstaclePrefabs.Count)],
            spawnPoint, Quaternion.identity);
        Vector2 dir = (transform.position - spawnPoint).normalized;
        dir = Vector2.Perpendicular(dir);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        obstacle.transform.rotation = Quaternion.Euler(0, 0, angle);
        return obstacle;
    }

    private void OnRoadRecovered(OnTemporaryBlockedUnDirectedEdgeRecovered e) {
        if (e.fromNode.Equals(Node.Value) ) {
            if (Connection2Obstacle.ContainsKey(e.toNode))
            {
                Connection2Obstacle.Remove(e.toNode);
            }
            Vector3 spawnPoint = Neighbours2ConnectionLineMidPoints[e.toNode];
            spawnPoint = new Vector3(spawnPoint.x, spawnPoint.y, 0);

            foreach (Collider c in Physics.OverlapSphere(spawnPoint, 1))
            {
                if (c.gameObject.layer == LayerMask.NameToLayer("Obstacles")) {
                    Debug.Log(c.gameObject.name);
                    Destroy(c.transform.parent. gameObject);
                }
              
            }
        }

        
    }

    private void OnRoadBlocked(OnTemporaryBlockedUnDirectedEdgeAdded e) {
        if (e.fromNode.Equals(Node.Value) && !Connection2Obstacle.ContainsKey(e.toNode)) {
            //Debug.Log($"Blocked at position: {Neighbours2ConnectionLineMidPoints[e.toNode]}");
            Vector3 spawnPoint = Neighbours2ConnectionLineMidPoints[e.toNode];
            spawnPoint = new Vector3(spawnPoint.x, spawnPoint.y, 0);
            GameObject obstacle =SpawnRandomObstacle(spawnPoint);

          
            Connection2Obstacle.Add(e.toNode, obstacle);
            
            
        }
    }

    private void OnEnemyKilled(OnMapEnemyKilled e) {
        if (e.EnemyVertex.Value.Equals(Node.Value)) {
            UpdateNodeSprite();
        }
    }

    private void OnEnemyNodeMoveToNewVertex(OnEnemyNodeMoveToNewVertex e) {
        //move from this node
        if (e.OldVertex == Node) {
            UpdateNodeSprite();
            //Spawn A "Enemy" and let it move 
            GameObject enemy = movingEnemyPool.Allocate();
            enemy.transform.position = transform.position;
            enemy.GetComponent<MovingEnemy>().targetMoveVertex = e.NewVertex;
        }
    }


    private void OnNodeTypeChange(LevelType prevType, LevelType curType) {
        //this.GetComponent<SpriteRenderer>().sprite = levelTypeSirSprites[(int) curType];
    }

    public void UpdateNodeSprite() {
        this.GetComponent<SpriteRenderer>().sprite = levelTypeSirSprites[(int)Node.Value.LevelType.Value];
    }

    private void OnALevelSelected(GraphVertex prevNode, GraphVertex newSelectedLevel) {
        if (newSelectedLevel == null){ //floor 0 or load save data
            Debug.Log("Call OnPlayerMeet");
            if (Node.Value.PointOnMap.y == this.GetModel<IMapGenerationModel>().PathDepth) {
                OnPlayerMeet();
            }
        }
        else {
            if (prevNode == null && !newSelectedLevel.Value.Equals(Node.Value)) { //floor 0 selected
                //OnPlayerLeave();
                if (Node.Value.PointOnMap.y == this.GetModel<IMapGenerationModel>().PathDepth) {
                    OnPlayerLeave();
                }
            }

            if (prevNode!=null && prevNode.Value.Equals(Node.Value)) {
                Node.Neighbours.ForEach(node => {
                    if (!node.Value.Equals(newSelectedLevel.Value))
                    {
                        node.Value.LevelObject.GetComponent<LevelObject>().OnPlayerLeave();
                    }
                });
                this.GetSystem<ITimeSystem>().AddDelayTask(0.02f, () => {
                    OnPlayerMoveToNext();
                });

            }

            if (newSelectedLevel.Value.Equals(Node.Value)) {
                //selected this
                OnPlayerSelect();
                this.GetSystem<ITimeSystem>().AddDelayTask(0.05f, () => {
                    Node.Neighbours.ForEach(node => {
                        if (!Node.TemporaryBrokenConnections.Contains(node.Value)) {
                            //Debug.Log($"Select:{gameObject.name}, Meet:{node.LevelObject.gameObject.name}");
                            node.Value.LevelObject.GetComponent<LevelObject>().OnPlayerMeet();
                        }
                        else {
                            node.Value.LevelObject.GetComponent<LevelObject>().OnPlayerLeave();
                        }
                     
                    });
                });

            }

           
        } 
    }

    private void OnPlayerMeet() {
     
        animator.SetBool("PlayerReach", true);
        Interactable = true;
        //Debug.Log(gameObject.name +" "+  Interactable);
    }

    private void OnPlayerSelect() {
        animator.SetBool("PlayerComplete", true);
        Interactable = false;
    }

    private void OnPlayerLeave() {
        animator.SetBool("PlayerReach", false);
        animator.SetBool("PlayerComplete", false);
        GraphVertex currentSelectedNode = this.GetModel<IMapStateModel>().CurNode;
       
        Interactable = Node.Neighbours.Contains(currentSelectedNode);
    }

    private void OnPlayerMoveToNext() {
        animator.SetBool("PlayerComplete", false);
        GraphVertex currentSelectedNode = this.GetModel<IMapStateModel>().CurNode;

        Interactable = Node.Neighbours.Contains(currentSelectedNode);
    }
}
