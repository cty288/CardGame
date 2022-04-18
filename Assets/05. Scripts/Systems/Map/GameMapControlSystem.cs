using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.BindableProperty;
using UnityEngine;

namespace MainGame
{
    public interface IGameMapControlSystem : ISystem {

    }
    [ES3Serializable]
    public class GameMapControlSystem : AbstractSystem, IGameMapControlSystem, ICanRegisterAndLoadSavedData {
        [ES3Serializable] private List<PathEnemySystemNode> enemyNodes = new List<PathEnemySystemNode>();
        [ES3Serializable] private BindableProperty<int> timePassedInMapScene;
        protected override void OnInit() {
            enemyNodes = this.RegisterAndLoadFromSavedData("enemy_system_node", new List<PathEnemySystemNode>());
            timePassedInMapScene =
                this.RegisterAndLoadFromSavedData("time_passed_in_map", new BindableProperty<int>(0));
            this.RegisterEvent<OnNewMapGenerated>(OnNewMapGenerated);
            this.RegisterEvent<IGameTimeUpdateEvent>(OnGameTimeUpdate);
          
        }

        private void OnNewMapGenerated(OnNewMapGenerated e) {
            foreach (GraphVertex vertex in e.PathGraph.Vertices) {
                if (vertex.Value.LevelType == LevelType.Enemy) {
                    enemyNodes.Add(new PathEnemySystemNode(vertex));
                } 
            }
        }

        private void OnGameTimeUpdate(IGameTimeUpdateEvent e) {
            timePassedInMapScene.Value += e.TimePassed;
            if (this.GetModel<IGameStateModel>().GameState == GameState.Map) {
                //TODO: check alive
                foreach (PathEnemySystemNode enemyNode in enemyNodes) {
                    //Debug.Log("Enemy node update time");
                    enemyNode.OnMapTimePassed(timePassedInMapScene.Value);
                }
                timePassedInMapScene.Value = 0;
            }
        }

        
    }
}
