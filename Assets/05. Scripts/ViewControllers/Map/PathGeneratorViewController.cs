using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
            //Floor0Stage();
        }

        void CreatePathRoutine(Graph graph)
        {
            Random random = this.GetSystem<ISeedSystem>().RandomGeneratorRandom;
            Dictionary<GraphVertex, Vector3> vertexRealPos = new Dictionary<GraphVertex, Vector3>();
            int pathDepth = this.GetModel<IMapGenerationModel>().PathDepth;
            Dictionary<GraphVertex, List<GraphVertex>> graphWithAlreadyConnectedNodes =
                new Dictionary<GraphVertex, List<GraphVertex>>();

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
                }
              
                
               
            }


            for (int i = 0; i < graph.Vertices.Count; ++i) {
                if (graph.Vertices[i].NodeNeighbours.Count > 0) {
                    if (graph.Vertices[i].Value.Depth < 0) {
                        continue;
                    }
                    GraphVertex vertex = graph.Vertices[i];
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

                        graphWithAlreadyConnectedNodes[graph.Vertices[i]].Add(toNode);
                        graphWithAlreadyConnectedNodes[toNode].Add(graph.Vertices[i]);

                        Vector3 fromPos = vertexRealPos[vertex];
                        Vector3 toPos = vertexRealPos[toNode];
                        toPos += (toPos - fromPos).normalized * 1f;
                        fromPos += (fromPos - toPos).normalized * 1f;
                        /*
                        if (toNode.Value.PointOnMap.y != vertex.Value.PointOnMap.y) {
                            if (toNode.Value.PointOnMap.y > vertex.Value.PointOnMap.y) {
                               
                                fromPos +=  new Vector3(0, - 1.8f, 0);
                                toPos += new Vector3(0, 1.8f, 0); 
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            if (toNode.Value.PointOnMap.x> vertex.Value.PointOnMap.x)
                            {
                                fromPos += new Vector3(1.8f, 0, 0);
                                toPos += new Vector3(-1.8f, 0, 0);
                                
                            }
                            else
                            {
                                continue;
                            }
                        }

                        */
                        DrawConnectionLine(fromPos, toPos);
                     }
                }
            }

        }

        private void DrawConnectionLine(Vector3 fromPos, Vector3 toPos) {
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


