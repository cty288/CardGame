using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MainGame
{
    public struct OnNewMapGenerated
    {
        public Graph PathGraph;
    }

    public struct OnLevelSelected : IGameTimeUpdateEvent
    {
        public LevelObject LevelObject;
        public int TimePassed { get; set; }
    }

    public struct OnAddNode {
        public GraphVertex AddedNode;
    }

    public struct OnRemoveNode
    {
        public GraphVertex RemovedNode;
    }

    public struct OnAddDirectedEdge
    {
        public GraphVertex Start;
        public GraphVertex End;
    }
}