using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;
using UnityEngine.UIElements;

namespace MainGame
{
    [ES3Serializable]
    public class GraphVertex {
        
        [ES3Serializable]
        private List<float> costs;
        public List<float> Costs => costs;

        [ES3Serializable]
        private MapNode value;

       
        public MapNode Value => value;

        [ES3NonSerializable]
        private List<GraphVertex> neighbours;
        public List<GraphVertex> Neighbours => neighbours;


        /// <summary>
        /// For load/save
        /// </summary>
        [ES3Serializable]
        private List<MapNode> nodeNeighbours;
        public List<MapNode> NodeNeighbours => nodeNeighbours;

        public GraphVertex() {
            costs = new List<float>();
            neighbours = new List<GraphVertex>();
            nodeNeighbours = new List<MapNode>();
            this.value = null;
        }

        public GraphVertex(MapNode value, List<GraphVertex> neightbours) : this() {
            this.value = value;
            this.neighbours = neightbours;
        }
        public GraphVertex(MapNode value) : this() {
            this.value = value;
        }
    }

    [ES3Serializable]
    public class Graph: ICanSendEvent {


        [ES3Serializable]
        private List<GraphVertex> vertices;

        public List<GraphVertex> Vertices => vertices;

        public void LoadSavedGraph() {
            foreach (GraphVertex vertex in vertices) {
                foreach (MapNode mapNodeNeighbour in vertex.NodeNeighbours) {
                    foreach (GraphVertex graphVertex in vertices) {
                        if (graphVertex.Value.Equals(mapNodeNeighbour)) {
                            vertex.Neighbours.Add(graphVertex);
                            break;
                        }
                    }
                }
            }
        }
        public float Count {
            get {
                return vertices.Count;
            }
        }

        public Graph(): this(null) {

        }

        public Graph(List<GraphVertex> vertices) {
            if (vertices == null) {
                this.vertices = new List<GraphVertex>();
            }
            else {
                this.vertices = vertices;
            }
        }

        public void AddVertex(GraphVertex vertex) {
            vertices.Add(vertex);
            
            this.SendEvent<OnAddNode>(new OnAddNode(){AddedNode = vertex });
        }

        public void AddVertex(MapNode node) {
            AddVertex(new GraphVertex(node));
        }
        public void AddDirectedEdge(GraphVertex start, GraphVertex end, float cost) {
            start.Neighbours.Add(end);
            start.Costs.Add(cost);
            start.NodeNeighbours.Add(end.Value);
            this.SendEvent<OnAddDirectedEdge>(new OnAddDirectedEdge() {Start = start, End = end});
        }

        public void AddUnDirectedEdge(GraphVertex node1, GraphVertex node2, float cost1, float cost2)
        {
            AddDirectedEdge(node1, node2, cost1);
            AddDirectedEdge(node2, node1, cost2);
        }
        public void AddUnDirectedEdge(GraphVertex node1, GraphVertex node2, float cost) {
            AddUnDirectedEdge(node1, node2, cost, cost);
        }

        public GraphVertex FindVertexByValue(MapNode value) {
            foreach (GraphVertex node in vertices) {
                if (node.Value.Equals(value)) {
                    return node;
                }
                  
            }
            return null;
        }

        public GraphVertex FindVertexByPos(int x, int y) {
            foreach (GraphVertex vertex in vertices) {
                if (vertex.Value.PointOnMap == new Vector2Int(x, y)) {
                    return vertex;
                }
            }

            return null;
        }
        public bool RemoveVertex(MapNode value)
        {

            GraphVertex nodeToRemove = FindVertexByValue(value);
            if (nodeToRemove == null)  return false;


            this.SendEvent<OnRemoveNode>(new OnRemoveNode(){RemovedNode = nodeToRemove});
            vertices.Remove(nodeToRemove);
         
            foreach (GraphVertex vertex in vertices)
            {
                int index = vertex.Neighbours.IndexOf(nodeToRemove);
                if (index != -1) {
                    vertex.Neighbours.RemoveAt(index);
                    vertex.Costs.RemoveAt(index);
                    vertex.NodeNeighbours.RemoveAt(index);
                }
            }
            return true;
        }


        public IArchitecture GetArchitecture() {
            return CardGame.Interface;
        }
    }

}
