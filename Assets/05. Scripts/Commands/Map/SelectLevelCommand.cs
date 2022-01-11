using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Pool;
using UnityEngine;

namespace MainGame
{
    public class SelectLevelCommand : AbstractCommand<SelectLevelCommand>, ICanSaveGame {
        private LevelObject LevelObject;

        protected override void OnExecute() {
            if (LevelObject.Interactable) {
                LevelObject.Node.Visited = true;
                this.GetModel<IMapStateModel>().CurNode.Value = LevelObject.Node;
                this.SendEvent<IGameTimeUpdateEvent>(new OnSelectLevelTimePass() {
                    TimePassed = 15
                });
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
