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
                LevelObject.Node.Visited = true;
                this.GetModel<IMapStateModel>().CurNode.Value = LevelObject.Node;
                this.SendEvent<IGameTimeUpdateEvent>(new OnSelectLevelTimePass() {
                    TimePassed = 15
                });

                if (LevelObject.Node.NodeType.Value == LevelType.Elite || LevelObject.Node.NodeType.Value ==
                                                                       LevelType.Enemy
                                                                       || LevelObject.Node.NodeType.Value ==
                                                                       LevelType.Boss) {
                   // this.GetModel<IGameStateModel>().GameState.Value = GameState.Battle;
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
