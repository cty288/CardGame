using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

namespace MainGame
{
    public interface IGameTimeConfigModel : IModel {
        public float TimeSpeed { get; set; }
        public int GameStartTime { get; set; }
    }
    public class GameTimeConfigModel : AbstractModel, IGameTimeConfigModel
    {
        protected override void OnInit() {
            
        }

        public float TimeSpeed { get; set; } = 1;
        public int GameStartTime { get; set; } = 480;
    }
}
