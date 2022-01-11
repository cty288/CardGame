using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.BindableProperty;
using UnityEngine;

namespace MainGame
{
    public enum GameState {
        Menu,
        Map,
        Battle
    }
    public interface IGameStateModel : IModel {
        public BindableProperty<GameState> GameState { get; set; }
    }
    public class GameStateModel : AbstractModel, IGameStateModel, ICanRegisterAndLoadSavedData {
        protected override void OnInit() {
            GameState = this.RegisterAndLoadFromSavedData("game_state",
                new BindableProperty<GameState>(MainGame.GameState.Map));
        }

        public BindableProperty<GameState> GameState { get; set; }
    }
}
