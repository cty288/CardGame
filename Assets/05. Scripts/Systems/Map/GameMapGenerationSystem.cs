using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using MikroFramework.Architecture;
using MikroFramework.Serializer;
using MikroFramework.TimeSystem;
using UnityEngine;
using Random = System.Random;


namespace MainGame
{
    public interface IGameMapGenerationSystem : ISystem {
        GraphVertex GetPathNodeAtDepthAndOrder(int depth, int order);
        public List<GraphVertex> GetPathNodeAtDepth(int depth);
        public List<GraphVertex> GetAllAvailableNodes();

        public Graph GetPathGraph();
    }

    public class GameMapGenerationGenerationSystem : AbstractSystem, IGameMapGenerationSystem, ICanRegisterAndLoadSavedData, ICanSaveGame {

        private Graph pathGraph;
        private IMapGenerationModel mapGenerationModel;

        public bool IsNodeTreeGenerated {
            get {
                return pathGraph.Count > 0;
            }
        }

        protected override void OnInit() {
            Debug.Log("Game map system init");
            mapGenerationModel = this.GetModel<IMapGenerationModel>();
            pathGraph = this.RegisterAndLoadFromSavedData("map_path", new Graph());
            if (!IsNodeTreeGenerated) {
                GeneratePath();
            }
            else {
                LoadSavedMap();
                Debug.Log("Load saved map");
            }
        }


        public Graph GeneratePath() {
            int pathWidth = mapGenerationModel.PathWidth;
            int pathDepth = mapGenerationModel.PathDepth;

            for (int i = 0; i <= pathWidth; i++) {
                for (int j = 0; j <= pathDepth; j++) {
                    //node generation part
                    bool generate = this.GetSystem<ISeedSystem>().MapRandom.Next(0,100) >= 50;
                    if (generate) {
                        bool canGenerate = true;
                        List<GraphVertex> generated = pathGraph.Vertices;
                        foreach (GraphVertex graphVertex in generated) {
                            if (Vector2.Distance(graphVertex.Value.PointOnMap, new Vector2(i, j)) <= 1) {
                               
                                canGenerate = this.GetSystem<ISeedSystem>().MapRandom.Next(0, 2) == 0 ;
                            }
                        }
                        if (canGenerate) {
                            pathGraph.AddVertex(new MapNode(LevelType.Enemy, i, j));
                        }
                      
                    }
                }
            }

          

            SetVertexLevelType();

            List<List<float>> distances = new List<List<float>>();
            List<List<float>> angles = new List<List<float>>();

            for (int i = 0; i < pathGraph.Count; i++) {
                distances.Add(new List<float>());
                angles.Add(new List<float>());
                for (int j = 0; j < pathGraph.Count; j++) {
                    float dist = MapNode.GetDistance(pathGraph.Vertices[i].Value, pathGraph.Vertices[j].Value);
                    distances[i].Add(dist);

                    float angle = MapNode.GetAngleInRadians(pathGraph.Vertices[i].Value, pathGraph.Vertices[j].Value);
                    angles[i].Add(angle);
                }

                //Distance -> index in list (ordered)
                List<KeyValuePair<float,int>> sortedConnection = distances[i]
                    .Select((dist, index) => new KeyValuePair<float, int>(dist, index)).OrderBy(pair => pair.Key)
                    .ToList();
                List<float> dists = sortedConnection.Select(x => x.Key).ToList();
                List<int> indexes = sortedConnection.Select(x => x.Value).ToList();

                List<float> angleOccuplied = new List<float>();
                GraphVertex currentVertex = pathGraph.Vertices[i];

                int maxNumConnect;

                if (currentVertex.Value.Depth == 0 || currentVertex.Value.Depth == pathDepth) {
                    maxNumConnect = 1;
                }else {
                    maxNumConnect = this.GetSystem<ISeedSystem>().MapRandom.Next(2, 5);
                }
                
                

                
                int numConnected = currentVertex.Neighbours.Count;

                if (numConnected < maxNumConnect) {
                    for (int j = 1; j < dists.Count; j++)
                    {
                        //No connections on the same depth when the current vertex is on the first or last depth
                        if ((currentVertex.Value.Depth == 0 || currentVertex.Value.Depth == pathDepth) &&
                            pathGraph.Vertices[indexes[j]].Value.Depth == currentVertex.Value.Depth)
                        {
                            continue;
                        }

                        //no overlap angles and distance not too far away
                        if (!angleOccuplied.Contains(angles[i][indexes[j]]) && 
                            Vector2.Distance(currentVertex.Value.PointOnMap, pathGraph.Vertices[indexes[j]].Value.PointOnMap)<=5)
                        {
                            
                            //angle limits
                            float angle = angles[i][indexes[j]] * 180 / Mathf.PI;
                            bool angleTooClose = false;
                            foreach (float a in angleOccuplied) {
                                if (Mathf.Abs(angle - a) < 45) {
                                    angleTooClose = true;
                                }
                            }
                            if(angleTooClose) continue;


                            //not too tilted
                            if ((Mathf.Abs(angle) % 90) <= this.GetSystem<ISeedSystem>().MapRandom.Next(15, 30)) {
                                
                                if (pathGraph.Vertices[indexes[j]].Neighbours.Count <= 4)
                                {
                                   
                                    //eliminate road intersections
                                    if (!HaveIntersectionWithOtherConnections(currentVertex.Value.PointOnMap,
                                            pathGraph.Vertices[indexes[j]].Value.PointOnMap)) {
                                        angleOccuplied.Add(angles[i][indexes[j]]);
                                        pathGraph.AddUnDirectedEdge(
                                            pathGraph.Vertices[i],
                                            pathGraph.Vertices[indexes[j]], dists[j]);
                                        numConnected++;
                                    }
                                 
                                }
                              
                            }
                           
                        }

                        if (numConnected >= maxNumConnect)
                        {
                            break;
                        }
                    }
                
                }
            }

            //create a  "final" node to make the pathfinding easier.
            GraphVertex finalVertex = new GraphVertex(new MapNode(LevelType.Nothing, pathWidth / 2, -1));
            pathGraph.AddVertex(finalVertex);
            List<GraphVertex> topVertices = GetPathNodeAtDepth(0);
            foreach (GraphVertex vertex in topVertices) {
                pathGraph.AddUnDirectedEdge(vertex, finalVertex, 0);
            }

            //start pathfinding to eliminite nodes that don't connect to exit
            List<GraphVertex> verticesCheckSuccess = new List<GraphVertex>();
            List<GraphVertex> verticesCheckFailed = new List<GraphVertex>();

            foreach (GraphVertex vertex in pathGraph.Vertices) {
                if (!verticesCheckSuccess.Contains(vertex) && !verticesCheckFailed.Contains(vertex)) {
                    BasicAStarPathFinder pathFinder = new BasicAStarPathFinder();
                    pathFinder.HeuristicCost = MapNode.GetManhattanCost;
                    pathFinder.NodeTraversalCost = MapNode.GetEuclideanCost;

                    pathFinder.Initialize(vertex, finalVertex);
                    PathFindingStatus status = pathFinder.Step();
                    while (status == PathFindingStatus.Running) {
                        status = pathFinder.Step();
                    }

                    if (status == PathFindingStatus.Success) {
                        AddAllNeighboursToList(vertex, verticesCheckSuccess);
                    }

                    if (status == PathFindingStatus.Failed) {
                        AddAllNeighboursToList(vertex, verticesCheckFailed);
                    }
                }
            }

            //remove all failed vertex
            foreach (GraphVertex vertex in verticesCheckFailed) {
                pathGraph.RemoveVertex(vertex.Value);
                Debug.Log($"Removed unconnected node: {vertex.Value.PointOnMap}");
            }


            //ES3.Save("map_path",pathGraph);
            this.SendEvent<OnNewMapGenerated>(new OnNewMapGenerated() { PathGraph = pathGraph });
            this.GetSystem<ITimeSystem>().AddDelayTask(0.1f, () => {
                this.SendEvent<OnMapLoaded>(new OnMapLoaded() {PathGraph = pathGraph});
                Debug.Log("Sent event");
                this.SaveGame();
            });


            Debug.Log("Tree Built");

            return pathGraph;
            // StartCoroutine(CreatePathRoutine());
        }

        private void AddAllNeighboursToList(GraphVertex vertex, List<GraphVertex> list) {
            if (!list.Contains(vertex)) {
                list.Add(vertex);
                foreach (GraphVertex vertexNeighbour in vertex.Neighbours) {
                    if (!list.Contains(vertexNeighbour)) {
                        AddAllNeighboursToList(vertexNeighbour, list);
                    }
                }
            }
        }
        bool HaveIntersectionWithOtherConnections(Vector2Int p1, Vector2Int p2)
        {
           
           
            foreach (GraphVertex vertex in pathGraph.Vertices) {
                foreach (GraphVertex vertexNeighbour in vertex.Neighbours) {
                    Vector2Int c =vertexNeighbour.Value.PointOnMap;
                    Vector2Int d = vertex.Value.PointOnMap;

                    if (p1 == c || p1 == d || p2 == c || p2 == d) {
                        continue;
                    }
                    if (CheckCross(p1, p2, c, d) && CheckCross(c, d, p1, p2)) {
                        return true;
                    }
                }
            }
           

            return false;
        }

        
        private bool CheckCross(Vector2Int a, Vector2Int b, Vector2Int c, Vector2Int d)
        {
            Vector2Int v1 = new Vector2Int();
            Vector2Int v2 = new Vector2Int();
            Vector2Int v3 = new Vector2Int();

            v1.x = c.x - b.x;
            v1.y = c.y - b.y;

            v2.x = d.x- b.x;
            v2.y =d.y -b.y;

            v3.x = a.x - b.x;
            v3.y = a.y -b.y;

            return (CrossMul(v1, v3) * CrossMul(v2, v3) < 0);

        }
      
      
        private float CrossMul(Vector2Int pt1, Vector2Int pt2)
        {
            return pt1.x * pt2.y - pt1.y * pt2.x;
        }

      
        private void SetVertexLevelType() {
            List<GraphVertex> vertices = pathGraph.Vertices;
            Random random = this.GetSystem<ISeedSystem>().MapRandom;
            List<float> possibilities = mapGenerationModel.NormalLevelPossibilityValues;
            foreach (GraphVertex graphVertex in vertices) {
                LevelType type = LevelType.Nothing;
                do {
                    float lowerBound = 0;
                    int levelChance = random.Next(0, 100);
                    for (int i = 0; i < possibilities.Count; i++)
                    {

                        if (levelChance >= lowerBound && levelChance < lowerBound + possibilities[i])
                        {
                            type = mapGenerationModel.NormalLevelTypes[i];
                            break;
                        }

                        lowerBound += possibilities[i];
                    }
                } while (!CheckNodeTypeValidity(graphVertex.Value.PointOnMap.x, graphVertex.Value.PointOnMap.y, type));

                graphVertex.Value.LevelType.Value = type;
            }
           
        }

        private bool CheckNodeTypeValidity(int x, int y, LevelType type) {
            if (type == LevelType.Enemy || type == LevelType.Nothing || type == LevelType.Rest ||
                type == LevelType.Treasure) {
                return true;
            }

            GraphVertex vertex = pathGraph.FindVertexByPos(x, y);
            foreach (GraphVertex neighbour in vertex.Neighbours) {
                if (vertex.Value.LevelType == type) {
                    return false;
                }
            }

            return true;
        }
        private void LoadSavedMap() {
            pathGraph.LoadSavedGraph();
            this.GetSystem<ITimeSystem>().AddDelayTask(0.1f, () => {
                this.SendEvent<OnMapLoaded>(new OnMapLoaded() { PathGraph = pathGraph });
            });
        }

        public GraphVertex GetPathNodeAtDepthAndOrder(int depth, int order)
        {
            Debug.Log("GetPathNodeAtDepthAndOrder");
            return pathGraph.FindVertexByPos(order, depth);
        }

        public List<GraphVertex> GetPathNodeAtDepth(int depth)
        {
            List<GraphVertex> allNodes = pathGraph.Vertices;
             Random random = this.GetSystem<ISeedSystem>().MapRandom;
            List<GraphVertex> retResult = new List<GraphVertex>();
            foreach (GraphVertex pathNode in allNodes)
            {
                if (pathNode != null && pathNode.Value.Depth == depth)
                {
                    retResult.Add(pathNode);
                }
            }

            return retResult;
        }
      
        public List<GraphVertex> GetAllAvailableNodes()
        {
            List<GraphVertex> allNodes = new List<GraphVertex>();
            foreach (GraphVertex node in pathGraph.Vertices)
            {
              
                if (node != null)
                {
                    allNodes.Add(node);
                }
                
            }

            return allNodes;
        }

        public Graph GetPathGraph() {
            return pathGraph;
        }

        #region Obsolete

        /*
      

      

       private void SortNodesList(List<PathNode> nodes) {
       for (int i = 0; i < nodes.Count - 1; i++)
       {
           for (int j = i + 1; j < nodes.Count; j++)
           {

               if (nodes[i].Order > nodes[j].Order)
               {
                   PathNode temp = nodes[i];
                   nodes[i] = nodes[j];
                   nodes[j] = temp;
               }
           }
       }
   }
       private int GetConnectedNodeNumber(int depth) {
           int pathDepth = mapGenerationModel.PathDepth;
           Random random = this.GetSystem<ISeedSystem>().MapRandom;

           int distanceFromStart = (pathDepth / 2) - Mathf.Abs(depth - (pathDepth / 2));

           float frac = (float)distanceFromStart / (pathDepth / 2);
           //frac: 0 - 1
           int chance = random.Next(0, 100);

           if (depth >= 1 && depth != pathDepth - 2)
           {

               if (frac >= 0 && frac < 0.3)
               {

                   if (chance >= 0 && chance < 60)
                   {
                       return 1;
                   }
                   else if (chance >= 60 && chance < 85)
                   {
                       return 2;
                   }
                   else if (chance >= 85 && chance < 93)
                   {
                       return 0;
                   }

                   return 2;


               }
               else if (frac >= 0.3 && frac < 0.6)
               {
                   if (chance >= 0 && chance < 55)
                   {
                       return 1;
                   }
                   else if (chance >= 55 && chance < 85)
                   {
                       return 2;
                   }
                   else if (chance >= 85 && chance < 90)
                   {
                       return 0;
                   }

                   return 3;
               }
               else if (frac >= 0.6 && frac < 0.9)
               {

                   if (chance >= 0 && chance < 50)
                   {
                       return 1;
                   }
                   else if (chance >= 50 && chance < 85)
                   {
                       return 2;
                   }
                   else if (chance >= 85 && chance < 88)
                   {
                       return 0;
                   }

                   return 3;
               }

               else
               {
                   if (chance >= 0 && chance < 40)
                   {
                       return 1;
                   }
                   else if (chance >= 50 && chance <= 80)
                   {
                       return 2;
                   }

                   return 3;
               }

           }
           else
           {
               if (depth < 1)
               {
                   return random.Next(4, 7);
               }
               else
               {
                   return 1;
               }

           }

       }

       private int GenerateConnectedNodeIndex(int minimumBound, int maxBound, List<PathNode> existingNodes) {
           Random random = this.GetSystem<ISeedSystem>().MapRandom;
           if (existingNodes.Count > 0 && existingNodes[existingNodes.Count - 1].Order >= minimumBound)
           {
               int ran = random.Next(0, 100);
               if (ran <= 95)
               {
                   return existingNodes[existingNodes.Count - 1].Order;
               }
           }
           return random.Next(minimumBound, maxBound + 1);

       }



       private PathNode CheckNodeExist(int order, List<PathNode> existingNodes)
       {
           foreach (PathNode existingNode in existingNodes)
           {
               if (existingNode.Order == order)
               {
                   return existingNode;
               }
           }

           return null;
       }

       private PathNode CreateNewNode(int depth, int order) {
           Random random = this.GetSystem<ISeedSystem>().MapRandom;
           LevelType type = LevelType.Nothing;

           if (depth == 1)
           {
               type = LevelType.Rest;
           }
           else if (depth == treasureLevel)
           {
               type = LevelType.Treasure;
           }
           else if (depth == mapGenerationModel.PathDepth)
           {
               type = LevelType.Enemy;
           }
           else {
               float lowerBound = 0;

               List<float> possibilities = mapGenerationModel.NormalLevelPossibilityValues;
               int levelChance = random.Next(0, 100);

               for (int i = 0; i < possibilities.Count; i++) {

                   if (levelChance >= lowerBound && levelChance < lowerBound + possibilities[i]) {
                       type = mapGenerationModel.NormalLevelTypes[i];
                       break;
                   }

                   lowerBound += possibilities[i];

               }

           }



           return PathNode.Allocate(depth, order, type);
       }

       void BuildBossLevel() {

           pathGraph[0][mapGenerationModel.PathWidth / 2] = PathNode.Allocate(0, mapGenerationModel.PathWidth / 2,
               LevelType.Boss);
       }

      */

        #endregion

    }
}
