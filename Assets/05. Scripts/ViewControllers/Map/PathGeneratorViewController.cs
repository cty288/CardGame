using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dreamteck.Splines;
using MainGame;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.Event;
using MikroFramework.Pool;
using MikroFramework.Singletons;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

namespace MainGame
{
    public class PathGeneratorViewController : AbstractMikroController<CardGame>, ISingleton
    {
        public void OnSingletonInit()
        {

        }
        public static PathGeneratorViewController Singleton {
            get {
                return SingletonProperty<PathGeneratorViewController>.Singleton;
            }
        }

        //public static PathGeneratorViewController Singleton;

        [Range(0f, 10.0f)]
        [SerializeField]
        private float cellXInterval = 1.0f;
        public float CellXInterval => cellXInterval;

        [Range(0f, 20f)]
        [SerializeField]
        private float cellYInterval = 1.0f;
        public float CellYInterval => cellYInterval;

        [SerializeField] private float xOffset = 1;
        [SerializeField] private float yOffset = 2;


        [SerializeField]
        private GameObject levelObjectPrefab;

        [SerializeField]
        private GameObject line;

        [SerializeField]
        private Transform pathParent;

        [SerializeField]
        private Transform connectionParent;

        public Transform startLocation;

        
        private void Start()
        {
            this.RegisterEvent<OnMapLoaded>(OnNewMapGenerated).UnRegisterWhenGameObjectDestroyed(gameObject);
            
            
            GameObjectPoolManager.Singleton.CreatePool(levelObjectPrefab, 200, 500).NumObjDestroyPerFrame = 200;
            

            GameObjectPoolManager.Singleton.CreatePool(line, 100, 400);


        }

        private void Update() {
            if (!this.GetSystem<IMapAnimationControlSystem>().IsBlockable) {
                CheckClick();
            }
            
        }

        private void CheckClick() {
            Camera cam = Camera.main;

            if (Input.GetMouseButtonUp(0))
            {
                RaycastHit2D ray = Physics2D.GetRayIntersection(cam.ScreenPointToRay(Input.mousePosition));

                Collider2D collider = ray.collider;

                if (collider != null && collider.gameObject.CompareTag("MapItem"))
                {
                    LevelObject level = collider.GetComponent<LevelObject>();
                    
                    this.SendCommand<SelectLevelCommand>(SelectLevelCommand.Allocate(level));
                }

            }
        }

        private void OnNewMapGenerated(OnMapLoaded e) {
            Debug.Log("Event received");
            CreatePathRoutine(e.PathGraph);
            StartCoroutine(SpawnBuildings());
            //Floor0Stage();
        }

        [SerializeField] private List<GameObject> buildingPrefabs;
        private IEnumerator SpawnBuildings() {
            yield return null;
            Random random = this.GetSystem<ISeedSystem>().RandomGeneratorRandom;
            for (float i = -5; i < this.GetModel<IMapGenerationModel>().PathDepth * cellYInterval; i+= cellYInterval) {
                for (float j = -15; j < this.GetModel<IMapGenerationModel>().PathWidth * cellXInterval; j+=cellXInterval) {
                    int index = random.Next(0, buildingPrefabs.Count);
                    GameObject randomPrefab = buildingPrefabs[index];
                    float radius = randomPrefab.GetComponent<BoxCollider>().size.x / 2;
                    radius *= randomPrefab.transform.localScale.x;
                    //Debug.Log(radius);
                    bool isCollide = Physics.OverlapSphere(new Vector3(i, j, 0), radius).ToList().Any();
                    if (!isCollide ){
                        Instantiate(randomPrefab, new Vector3(i, j, -0.18f), Quaternion.Euler(-90, 0, 0));
                    }
                }
            }
           
        }

        void CreatePathRoutine(Graph graph)
        {
            Random random = this.GetSystem<ISeedSystem>().RandomGeneratorRandom;
            Dictionary<GraphVertex, Vector3> vertexRealPos = new Dictionary<GraphVertex, Vector3>();
            int pathDepth = this.GetModel<IMapGenerationModel>().PathDepth;
            Dictionary<GraphVertex, List<GraphVertex>> graphWithAlreadyConnectedNodes =
                new Dictionary<GraphVertex, List<GraphVertex>>();
            Dictionary<GraphVertex, GameObject> vertex2LevelObj = new Dictionary<GraphVertex, GameObject>();

            for (int i = 0; i < graph.Vertices.Count; ++i)
            {
                if (graph.Vertices[i].Value.Depth >= 0) {
                    graphWithAlreadyConnectedNodes.Add(graph.Vertices[i], new List<GraphVertex>());
                    Vector3 pos = Vector3.zero;
                    pos.x = graph.Vertices[i].Value.PointOnMap.x * cellXInterval + ((float)((random.NextDouble() * 2 - 1))) * xOffset;
                    pos.y = (pathDepth - graph.Vertices[i].Value.PointOnMap.y) * cellYInterval + ((float)((random.NextDouble() * 2 - 1))) * yOffset;
                    pos.z = 0.0f;
                    vertexRealPos.Add(graph.Vertices[i], new Vector3(pos.x, pos.y, -0.1f));

                    GameObject obj = GameObjectPoolManager.Singleton.Allocate(levelObjectPrefab);

                    obj.transform.position = pos;
                    obj.transform.SetParent(pathParent);
                    obj.GetComponent<LevelObject>().Node = graph.Vertices[i];
                    obj.GetComponent<LevelObject>().LevelType = graph.Vertices[i].Value.LevelType;
                    graph.Vertices[i].Value.LevelObject = obj;
                    vertex2LevelObj.Add(graph.Vertices[i], obj);
                }

            }


            for (int i = 0; i < graph.Vertices.Count; ++i) {
                if (graph.Vertices[i].NodeNeighbours.Count > 0) {
                    if (graph.Vertices[i].Value.Depth < 0) {
                        continue;
                    }
                    GraphVertex vertex = graph.Vertices[i];
                    LevelObject fromVertextLevelObj = vertex2LevelObj[vertex].GetComponent<LevelObject>();
                    

                    GraphVertex[] vertices = vertex.Neighbours.ToArray();
                    if (graph.Vertices[i].Value.Depth == 0) {
                        DrawConnectionLine(vertexRealPos[vertex], vertexRealPos[vertex] + new Vector3(0,100,0));
                    }
                   
                    //set x/y offset according to direction
                    foreach (GraphVertex toNode in vertices) {
                        if (toNode.Value.Depth < 0) {
                            continue;
                        }

                        if (graphWithAlreadyConnectedNodes[graph.Vertices[i]].Contains(toNode)) {
                            continue;
                        }
                        LevelObject toVertextLevelObj = vertex2LevelObj[toNode].GetComponent<LevelObject>();
                        graphWithAlreadyConnectedNodes[graph.Vertices[i]].Add(toNode);
                        graphWithAlreadyConnectedNodes[toNode].Add(graph.Vertices[i]);

                        Vector3 fromPos = vertexRealPos[vertex];
                        Vector3 toPos = vertexRealPos[toNode];
                        toPos += (toPos - fromPos).normalized * 1f;
                        fromPos += (fromPos - toPos).normalized * 1f;
                        SplineComputer line = DrawConnectionLine(fromPos, toPos).GetComponent<SplineComputer>();

                        SplinePoint[] points = line.GetPoints();
                        Vector3 midPoint = points[points.Length / 2].position;

                        fromVertextLevelObj.Neighbours2ConnectionLineMidPoints.Add(toNode.Value, midPoint);
                        toVertextLevelObj.Neighbours2ConnectionLineMidPoints.Add(vertex.Value, midPoint);
                    }
                }
            }

        }

        private GameObject DrawConnectionLine(Vector3 fromPos, Vector3 toPos) {
            Random random = this.GetSystem<ISeedSystem>().RandomGeneratorRandom;
            SplineComputer line = GameObjectPoolManager.Singleton.Allocate(this.line).GetComponent<SplineComputer>();
            int numMiddleNodes = random.Next(1, 4);
            float progress = 0;
            float progressPerStep = 1 / (numMiddleNodes + 1f);
            float curve = 0;
            List<SplinePoint> points = new List<SplinePoint>();

            points.Add(new SplinePoint(fromPos));
            for (int j = 0; j < numMiddleNodes; j++)
            {
                progress += progressPerStep;
                curve += ((float)(random.NextDouble() * 2) - 1f) * 0.5f;
                curve = Mathf.Clamp(curve, -1f, 1f);
                Vector2 curvePoint = (toPos - fromPos) * (progress) + fromPos;

                points.Add(new SplinePoint(curvePoint +
                                           Vector2.Perpendicular((toPos - fromPos).normalized) * curve));
            }
            points.Add(new SplinePoint(toPos));
            line.SetPoints(points.ToArray());

            for (int j = 0; j < line.pointCount; j++)
            {
                line.SetPointSize(j, 0.3f);
            }
            line.transform.SetParent(connectionParent);
            line.transform.localPosition = new Vector3(line.transform.localPosition.x,
                line.transform.localPosition.y, -11.95f);
            return line.gameObject;
        }

        }
    

        /*
        private void Floor0Stage() {
          //  yield return new WaitForSeconds(0.1f);
            List<PathNode> level0Nodes = this.GetSystem<IGameMapGenerationSystem>()
                .GetPathNodeAtDepth(this.GetModel<IMapGenerationModel>().PathDepth);
              
            foreach (PathNode level0Node in level0Nodes) {
              
                level0Node.LevelObject.GetComponent<LevelObject>().OnPlayerMeet();
            }
        }*/


    }


