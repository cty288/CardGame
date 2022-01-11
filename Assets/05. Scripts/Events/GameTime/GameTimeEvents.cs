using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MainGame
{
    public interface IGameTimeUpdateEvent {
        int TimePassed { get; set; }
    }

    public struct OnSelectLevelTimePass : IGameTimeUpdateEvent {
        public int TimePassed { get; set; }
    }
    
}
