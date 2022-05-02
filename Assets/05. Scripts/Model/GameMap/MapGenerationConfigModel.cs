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

        public int EnemyMovePossibilityPerMinutePassed { get; }

    }
    public class MapGenerationConfigModel : AbstractModel, IMapGenerationConfigModel {
        public int PathDepth { get; } = 15;
        public int PathWidth { get; } = 20;
        public Dictionary<LevelType, float> NormalLevelPossibilities { get; } = new Dictionary<LevelType, float>() {
            {LevelType.Unknown, 19},
            {LevelType.Nothing, 41},
            {LevelType.Rest, 3},
            {LevelType.Enemy, 35},
            {LevelType.Elite, 100}
        };

        public int EnemyMovePossibilityPerMinutePassed { get; } = 1;


        protected override void OnInit() {
            
        }

       
    }
}
