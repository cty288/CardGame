using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Event;
using UnityEngine;

namespace MainGame
{
    public class GameSceneManager : AbstractMikroController<CardGame> {
        [SerializeField] private Camera[] stateCameras;

        private void Start() {
            this.GetModel<IGameStateModel>().GameState.RegisterWithInitValue(OnGameStateChange)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void OnGameStateChange(GameState prevState, GameState curState) {
            
            int prevIndex = (int) prevState;
            int curIndex = (int) curState;

            stateCameras[prevIndex].enabled = false;
            stateCameras[curIndex].enabled = true;
        }
    }
}
