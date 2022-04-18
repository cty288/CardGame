using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

namespace MainGame
{
    public class EnemyAStarPathFinder : AbstractAStarPathFinder, ICanGetModel
    {
        public override bool AlgorithmSpecificIsValidNeighbourCheck(GraphVertex vertex) {
            if (vertex.Value.LevelType == LevelType.Nothing || vertex.Value.LevelType == LevelType.Rest) {
                return true;
            }

         

            return false;
        }

        public IArchitecture GetArchitecture() {
            return CardGame.Interface;
        }
    }
}
