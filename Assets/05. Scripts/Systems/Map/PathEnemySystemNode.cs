using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

namespace MainGame
{
    [ES3Serializable]
    public class PathEnemySystemNode : ICanSendEvent, ICanRegisterEvent, ICanRegisterAndLoadSavedData, ICanGetModel {
        [ES3Serializable] private int timeSinceLastMove = 0;

        [ES3Serializable]
        private GraphVertex currentGraphVertex;

        public PathEnemySystemNode(){}

        public PathEnemySystemNode(GraphVertex location) {
            this.currentGraphVertex = location;
        }

        public void RefreshVertex() {
            currentGraphVertex = this.GetSystem<IGameMapGenerationSystem>().GetPathNodeAtDepthAndOrder(
                currentGraphVertex.Value.Depth,
                currentGraphVertex.Value.Order);
        }
        public void OnMapTimePassed(int timePassed) {
            timeSinceLastMove += timePassed;
            if (this.GetSystem<ISeedSystem>().RandomGeneratorRandom.Next(0, 100) <=
                this.GetModel<IMapGenerationModel>().EnemyMovePossibilityPerMinutePassed * timeSinceLastMove) {
                
                //Debug.Log("Enemy move");


                GraphVertex playerCurrentVertex = this.GetModel<IMapStateModel>().CurNode.Value;


                //If player is near this enemy, then pathfinding to the player; otherwise, just random wandering
                if (Vector2.Distance(playerCurrentVertex.Value.PointOnMap, currentGraphVertex.Value.PointOnMap) <= 3
                    && this.GetSystem<ISeedSystem>().RandomGeneratorRandom.Next(0,100) <=50) {
                    Debug.Log("Move Towards Player");
                    EnemyMoveDestination(playerCurrentVertex);
                }
                else {

                    List<GraphVertex> availablePlaces = new List<GraphVertex>();
                    foreach (GraphVertex graphVertex in currentGraphVertex.Neighbours) {
                        if (graphVertex.Value.LevelType == LevelType.Nothing ||
                            graphVertex.Value.LevelType == LevelType.Rest) {
                            availablePlaces.Add(graphVertex);
                        }
                    }
                    //Debug.Log($"Available Places Empty: {availablePlaces.Count}");
                    if (availablePlaces.Count > 0) {
                        EnemyMoveDestination(
                            availablePlaces[
                                this.GetSystem<ISeedSystem>().RandomGeneratorRandom.Next(0, availablePlaces.Count)]);
                    }
                }

            }
         
        }
        //TODO: Can't move to player's position for now, but maybe can move in the future
        private void EnemyMoveDestination(GraphVertex targetPosition) {
            EnemyAStarPathFinder pathFinder = new EnemyAStarPathFinder();
            pathFinder.HeuristicCost = MapNode.GetManhattanCost;
            pathFinder.NodeTraversalCost = MapNode.GetEuclideanCost;
            pathFinder.Initialize(currentGraphVertex, targetPosition);
            PathFindingStatus status = pathFinder.Step();
            while (status==PathFindingStatus.Running) {
                 status =  pathFinder.Step();
            }

            if (status == PathFindingStatus.Success) {
                Queue<GraphVertex> path = new Queue<GraphVertex>();
                PathFindingNode currentPathFindingNode = pathFinder.CurrentNode;
                while (currentPathFindingNode != null) {
                    path.Enqueue(pathFinder.CurrentNode.Vertex);
                    currentPathFindingNode = currentPathFindingNode.Parent;
                }

                GraphVertex newVertex = path.Dequeue();
                //TODO: maybe true in the future, then start battle automatically

                if (newVertex.Value.PointOnMap == this.GetModel<IMapStateModel>().CurNode.Value.Value.PointOnMap) {
                    Debug.Log(
                        $"Enemy: {currentGraphVertex.Value.PointOnMap} Move Failed: Meet Player");
                    return;
                }


                timeSinceLastMove = 0;
                Debug.Log(
                    $"Enemy: {currentGraphVertex.Value.PointOnMap} Move to {newVertex.Value.PointOnMap}");
                this.SendEvent<OnEnemyNodeMoveToNewVertex>(new OnEnemyNodeMoveToNewVertex() {
                    NewVertex = newVertex,
                    OldVertex = currentGraphVertex
                });
                currentGraphVertex = newVertex;
            }else if (status == PathFindingStatus.Failed) {
                Debug.Log(
                    $"Enemy: {currentGraphVertex.Value.PointOnMap} Move Failed");
            }

        }
        public IArchitecture GetArchitecture() {
            return CardGame.Interface;
        }
    }
}
