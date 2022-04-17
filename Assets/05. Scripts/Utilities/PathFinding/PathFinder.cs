using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MainGame
{
    //A* and PathFinder learned and adapted from https://prod.liveshare.vsengsaas.visualstudio.com/join?CB2964FFAA30F3F43F8A9EF551DD42275148
    public enum PathFindingStatus {
        Uninitialized,
        Success,
        Failed,
        Running
    }
    public abstract class PathFinder {
        public PathFindingStatus Status { get; protected set; } = PathFindingStatus.Uninitialized;

        public GraphVertex Start { get; protected set; }
        public GraphVertex End { get; protected set; } 

        public PathFindingNode CurrentNode { get; private set; }

        protected List<PathFindingNode> openList = new List<PathFindingNode>();
        protected List<PathFindingNode> closeList = new List<PathFindingNode>();

        public Action<PathFindingNode> OnChangeCurrentNode;
        public Action<PathFindingNode> OnAddToOpenList;
        public Action<PathFindingNode> OnAddToClosedList;
        public Action<PathFindingNode> OnFindDestination;

        public Action OnStarted;
        public Action OnRunning;
        public Action OnFailed;
        public Action OnSuccess;

        public bool Initialize(GraphVertex start, GraphVertex end) {
            if (Status == PathFindingStatus.Running) {
                return false;
            }

            Reset();

            Start = start;
            End = end;

            float H = HeuristicCost(start.Value, end.Value);

            PathFindingNode root = new PathFindingNode(Start, null, 0f, H);
            openList.Add(root);
            CurrentNode = root;

            OnChangeCurrentNode?.Invoke(CurrentNode);
            OnStarted?.Invoke();
            Status = PathFindingStatus.Running;
            return true;
        }

        public PathFindingStatus Step() {
            closeList.Add(CurrentNode);
            OnAddToClosedList?.Invoke(CurrentNode);

            if (openList.Count == 0) {
                Status = PathFindingStatus.Failed;
                OnFailed?.Invoke();
                return Status;
            }

            //get the lest cost element from the open list
            CurrentNode = GetLeastCostNode(openList);
            OnChangeCurrentNode?.Invoke(CurrentNode);

            openList.Remove(CurrentNode);

            //if the current node is the goal
            if (CurrentNode.Vertex.Value.Equals(End.Value)) {
                Status = PathFindingStatus.Success;
                OnFindDestination?.Invoke(CurrentNode);
                OnSuccess?.Invoke();
                return Status;
            }

            List<GraphVertex> neighbours = CurrentNode.Vertex.Neighbours;

            foreach (GraphVertex graphVertex in neighbours) {
                AlgorithmImplementation(graphVertex);
            }

            Status = PathFindingStatus.Running;
            OnRunning?.Invoke();
            return Status;
        }

        protected void Reset() {
            if (Status == PathFindingStatus.Running) {
                return;
            }

            CurrentNode = null;
            openList.Clear();
            closeList.Clear();
            Status = PathFindingStatus.Uninitialized;
        }
        public abstract void AlgorithmImplementation(GraphVertex vertex);
        public Func<MapNode, MapNode, float> HeuristicCost;
        public Func<MapNode, MapNode, float> NodeTraversalCost;


        protected PathFindingNode GetLeastCostNode(List<PathFindingNode> nodeList) {
            int bestIndex = 0;
            float bestPriority = nodeList[0].FCost;

            for (int i = 1; i < nodeList.Count; i++) {
                if (bestPriority > nodeList[i].FCost) {
                    bestPriority = nodeList[i].FCost;
                    bestIndex = i;
                }
            }
            return nodeList[bestIndex];
        }

        protected int IsInList(List<PathFindingNode> list,  MapNode vertex) {
            for (int i = 0; i < list.Count; i++) {
                if (list[i].Vertex.Value.Equals(vertex)) {
                    return i;
                }
            }

            return -1;
        }
    }
}
