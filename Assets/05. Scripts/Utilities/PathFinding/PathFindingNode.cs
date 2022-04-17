using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MainGame
{
    public class PathFindingNode {
        public PathFindingNode Parent { get; set; }
        public GraphVertex Vertex { get; private set; }
       
        public float FCost { get; private set; }
        public float GCost { get; private set; }

        public float HCost { get; private set; }

        public PathFindingNode(GraphVertex vertex, PathFindingNode parent, float gCost, float hCost) {
            this.Vertex = vertex;
            this.Parent = parent;
            this.HCost = hCost;
            SetGCost(gCost);
        }

        public void SetGCost(float cost) {
            GCost = cost;
            FCost = GCost + HCost;
        }
    }
}
