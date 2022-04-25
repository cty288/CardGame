using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Pool;
using MikroFramework.TimeSystem;
using UnityEngine;

namespace MainGame
{
    public class SelectLevelCommand : AbstractCommand<SelectLevelCommand>, ICanSaveGame {
        private LevelObject LevelObject;
        protected override bool AutoRecycle { get; } = false;

        protected override void OnExecute() {
           
            if (LevelObject.Interactable) {
                if (this.GetModel<IMapStateModel>().CurNode.Value!=null && this.GetModel<IMapStateModel>().CurNode.Value.TemporaryBrokenConnections
                    .Contains(LevelObject.Node.Value)) {
                    this.SaveGame();
                    this.RecycleToCache();
                    return;
                }


                LevelObject.Node.Value.Visited = true;
                this.GetModel<IMapStateModel>().CurNode.Value = LevelObject.Node;
                this.SendEvent<IGameTimeUpdateEvent>(new OnSelectLevelTimePass() {
                    TimePassed = 15
                });

                if (LevelObject.Node.Value.LevelType.Value == LevelType.Elite || LevelObject.Node.Value.LevelType.Value ==
                                                                       LevelType.Enemy
                                                                       || LevelObject.Node.Value.LevelType.Value ==
                                                                       LevelType.Boss) {
                   // this.GetModel<IGameStateModel>().GameState.Value = GameState.Battle;
                   //kill enemy for now
                   //temporary code
                   this.SendEvent<OnMapEnemyKilled>(new OnMapEnemyKilled(){EnemyVertex = LevelObject.Node});
                   this.GetModel<IGameStateModel>().TotalEnemyPassed.Value++;
                   this.SendEvent<IBattleEvent>(new OnEnemyLevelPassed() {LevelVertex = LevelObject.Node});

                    this.GetSystem<ITimeSystem>().AddDelayTask(0.3f, () => {
                        //this.SendEvent<IBattleEvent>(new OnEnterBattleScene());
                        this.RecycleToCache();
                    });
                }
                
                this.SaveGame();
            }
        }

        public static SelectLevelCommand Allocate(LevelObject levelObject) {
            SelectLevelCommand cmd =  SafeObjectPool<SelectLevelCommand>.Singleton.Allocate();
            cmd.LevelObject = levelObject;
            return cmd;
        }
    }
}
