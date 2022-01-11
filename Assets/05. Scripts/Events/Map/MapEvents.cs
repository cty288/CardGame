using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MainGame
{
    public struct OnNewMapGenerated
    {
        public List<List<PathNode>> NodeTree;
    }

    public struct OnLevelSelected : IGameTimeUpdateEvent
    {
        public LevelObject LevelObject;
        public int TimePassed { get; set; }
    }
}