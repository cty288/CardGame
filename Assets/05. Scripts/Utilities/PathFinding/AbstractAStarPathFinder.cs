using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MainGame
{
    public abstract class AbstractAStarPathFinder : PathFinder
   {
       public override void AlgorithmImplementation(GraphVertex vertex) {
           if (IsInList(closeList, vertex.Value) == -1) {
               //Get the cost of the node from its parent
               //G -> cost from the start till now
               //current node -> the current node that connected to vertex
               //To get g, we get G of currentNode + cost from currentNode to this vertex
               float G = CurrentNode.GCost + NodeTraversalCost(CurrentNode.Vertex.Value, vertex.Value);
               float H = HeuristicCost(vertex.Value, End.Value);

               int idInOpenList = IsInList(openList, vertex.Value);

               //add to open list
               if (idInOpenList == -1) {
                   PathFindingNode pfn = new PathFindingNode(vertex, CurrentNode, G, H);
                    openList.Add(pfn);
                    OnAddToOpenList?.Invoke(pfn);
               }else {
                    //already in the openlist -> check G cost
                    float oldG = openList[idInOpenList].GCost;
                    if (G < oldG) {
                        openList[idInOpenList].Parent = CurrentNode;
                        openList[idInOpenList].SetGCost(G);
                        OnAddToOpenList?.Invoke(openList[idInOpenList]);
                    }
                   
               }
           }
       }

       
   }

    public class BasicAStarPathFinder : AbstractAStarPathFinder {
      

        public override bool AlgorithmSpecificIsValidNeighbourCheck(GraphVertex currentVertex, GraphVertex checkingVertex) {
            return true;
        }
    }

}
