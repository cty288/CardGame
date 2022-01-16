using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.TimeSystem;
using UnityEngine;

namespace MainGame
{
    public interface IBattleSystem : ISystem {

    }
    //control the flow of the battle
    public class BattleSystem : AbstractSystem, IBattleSystem {
        private IBattleEventControlSystem eventControlSystem;
        private IGameStateModel gameStateModel;

        protected override void OnInit() {
            eventControlSystem = this.GetSystem<IBattleEventControlSystem>();
            gameStateModel = this.GetModel<IGameStateModel>();
            this.RegisterEvent<IBattleEvent>(OnEnterBattleScene);

            if (gameStateModel.GameState.Value == GameState.Battle) {
                this.SendEvent<IBattleEvent>(new OnEnterBattleScene()); //first enter game
            }

        }

        private void OnEnterBattleScene(IBattleEvent obj) {
            if (obj is OnEnterBattleScene) {
                this.GetSystem<ITimeSystem>().AddDelayTask(0.5f, () => {
                    this.GetSystem<IBattleCardDeckSystem>().DrawAllCharacterCards();
                });

            }
           
        }
    }
}
