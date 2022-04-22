using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

namespace MainGame
{
    public class ExitAStarPathFinder : AbstractAStarPathFinder, ICanGetModel {
        public override bool AlgorithmSpecificIsValidNeighbourCheck(GraphVertex currentVertex, GraphVertex checkingVertex) {
            if (checkingVertex.Value.Depth < 0) {
                return true;
            }

            if (currentVertex.TemporaryBrokenConnections.Contains(checkingVertex.Value)) {
                return false;
            }

           
            return true;
        }

        public IArchitecture GetArchitecture() {
            return CardGame.Interface;
        }
    }
    public class EnemyAStarPathFinder : AbstractAStarPathFinder, ICanGetModel
    {
        public override bool AlgorithmSpecificIsValidNeighbourCheck(GraphVertex currentVertex, GraphVertex checkingVertex) {
            //TODO: change rest
            if (checkingVertex.Value.Depth < 0) {
                return false;
            }
            if (currentVertex.TemporaryBrokenConnections.Contains(checkingVertex.Value)) {
                return false;
            }

            if (checkingVertex.Value.LevelType == LevelType.Nothing){ //|| checkingVertex.Value.LevelType == LevelType.Rest) {
                return true;
            }

          
         

            return false;
        }

        public IArchitecture GetArchitecture() {
            return CardGame.Interface;
        }
    }
}
