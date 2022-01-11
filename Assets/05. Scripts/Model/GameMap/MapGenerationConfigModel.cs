using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

namespace MainGame
{
    public interface IMapGenerationConfigModel : IModel {
        public int PathDepth { get; }
        public int PathWidth { get; }
        public Dictionary<LevelType, float> NormalLevelPossibilities { get; }

    }
    public class MapGenerationConfigModel : AbstractModel, IMapGenerationConfigModel {
        public int PathDepth { get; } = 20;
        public int PathWidth { get; } = 50;
        public Dictionary<LevelType, float> NormalLevelPossibilities { get; } = new Dictionary<LevelType, float>() {
            {LevelType.Unknown, 19},
            {LevelType.Nothing, 54},
            {LevelType.Rest, 3},
            {LevelType.Enemy, 20},
            {LevelType.Elite, 3}
        };

       



        protected override void OnInit() {
            
        }

       
    }
}
