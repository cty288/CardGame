using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.BindableProperty;
using UnityEngine;
using Random = System.Random;

namespace MainGame
{
    public interface IGameMapControlSystem : ISystem {

    }
    [ES3Serializable]
    public class GameMapControlSystem : AbstractSystem, IGameMapControlSystem, ICanRegisterAndLoadSavedData {
        [ES3Serializable] private List<PathEnemySystemNode> enemyNodes = new List<PathEnemySystemNode>();
        [ES3Serializable] private BindableProperty<int> timePassedInMapScene;

        private Graph mapGraph;
        protected override void OnInit() {
            enemyNodes = this.RegisterAndLoadFromSavedData("enemy_system_node", new List<PathEnemySystemNode>());
            timePassedInMapScene =
                this.RegisterAndLoadFromSavedData("time_passed_in_map", new BindableProperty<int>(0));
            
            this.RegisterEvent<OnNewMapGenerated>(OnNewMapGenerated);
            this.RegisterEvent<OnMapLoaded>(OnMapLoaded);
            this.RegisterEvent<IGameTimeUpdateEvent>(OnGameTimeUpdate);
            this.RegisterEvent<OnEnemyNodeMoveToNewVertex>(OnEnemyNodeMoveToNewPlace);
            this.RegisterEvent<OnMapEnemyKilled>(OnEnemyKilled);
        }

        private void OnEnemyKilled(OnMapEnemyKilled e) {
            enemyNodes.Remove(enemyNodes.Find(node => node.CurrentGraphVertex.Value.Equals(e.EnemyVertex.Value)));
            e.EnemyVertex.Value.LevelType.Value = LevelType.Nothing;
        }

        private void OnMapLoaded(OnMapLoaded obj) {
            UpdateEnemyNodes();
        }

        private void UpdateEnemyNodes() {
            foreach (PathEnemySystemNode enemyNode in enemyNodes) {
                enemyNode.RefreshVertex();
            }
        }

        private void OnEnemyNodeMoveToNewPlace(OnEnemyNodeMoveToNewVertex e) {
            e.OldVertex.Value.LevelType.Value = LevelType.Nothing;
            e.NewVertex.Value.LevelType.Value = LevelType.Enemy;
        }

        private void OnNewMapGenerated(OnNewMapGenerated e) {
            foreach (GraphVertex vertex in e.PathGraph.Vertices) {
                if (vertex.Value.LevelType == LevelType.Enemy) {
                    enemyNodes.Add(new PathEnemySystemNode(vertex));
                } 
            }
        }

        private void OnGameTimeUpdate(IGameTimeUpdateEvent e) {
            mapGraph = this.GetSystem<IGameMapGenerationSystem>().GetPathGraph();
            timePassedInMapScene.Value += e.TimePassed;
            Random random = this.GetSystem<ISeedSystem>().RandomGeneratorRandom;
            if (this.GetModel<IGameStateModel>().GameState == GameState.Map) {
                foreach (PathEnemySystemNode enemyNode in enemyNodes) {
                    //Debug.Log("Enemy node update time");
                    enemyNode.OnMapTimePassed(timePassedInMapScene.Value);
                }

                //break roads
                if (mapGraph != null) {
                    foreach (GraphVertex vertex in mapGraph.Vertices) {
                        if (vertex.Value.Depth <= 0) {
                            continue;
                        }
                        //restore first
                        if (vertex.TemporaryBrokenConnections.Count > 0) {
                            foreach (MapNode brokenConn in vertex.TemporaryBrokenConnections) {
                                int restore = random.Next(0, 100);
                                if (restore <= timePassedInMapScene) {
                                    mapGraph.TemoporaryRestoreConnectionUnDirected(vertex,
                                        mapGraph.FindVertexByPos(brokenConn.Order, brokenConn.Depth));
                                    Debug.Log("Restore");
                                    break;
                                }
                            }
                        }

                        int breakChance = random.Next(0, 100);
                        if (breakChance <= timePassedInMapScene.Value / 3) {
                            //need break
                            GraphVertex brokeVertex = vertex.Neighbours[random.Next(0, vertex.Neighbours.Count)];
                            if(brokeVertex.Value.Depth<0){continue;}

                            mapGraph.TemoporaryRemoveConnectionUnDirected(vertex,brokeVertex);

                            ExitAStarPathFinder pathFinder = new ExitAStarPathFinder();
                            pathFinder.HeuristicCost = MapNode.GetManhattanCost;
                            pathFinder.NodeTraversalCost = MapNode.GetEuclideanCost;

                            pathFinder.Initialize(this.GetModel<IMapStateModel>().CurNode.Value,
                                mapGraph.FindVertexByPos(
                                    this.GetModel<IMapGenerationModel>().PathWidth / 2, -1));

                            PathFindingStatus status = pathFinder.Step();
                            while (status == PathFindingStatus.Running) {
                                status = pathFinder.Step();
                            }

                            if (status == PathFindingStatus.Failed)
                            {
                                mapGraph.TemoporaryRestoreConnectionUnDirected(vertex,brokeVertex);
                            }
                            else {
                                Debug.Log($"Break at: {vertex.Value.PointOnMap} to {brokeVertex.Value.PointOnMap}");
                            }
                        }
                    }



                }
                

                
                timePassedInMapScene.Value = 0;
            }
        }

        
    }
}
