using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Event;
using MikroFramework.TimeSystem;
using UnityEngine;

namespace MainGame
{
    public class GameSceneManager : AbstractMikroController<CardGame> {
        [SerializeField] private Camera[] stateCameras;

        private void Start() {
            this.GetModel<IGameStateModel>().GameState.RegisterOnValueChaned(OnGameStateChange)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            InitialSceneSetup();
        }

        private void InitialSceneSetup() {
            int curIndex = (int) this.GetModel<IGameStateModel>().GameState.Value;
            for (int i = 0; i < stateCameras.Length; i++) {
                stateCameras[i].enabled = curIndex == i;
            }
        }

        private void OnGameStateChange(GameState prevState, GameState curState) {
            this.GetSystem<ITimeSystem>().AddDelayTask(0.15f, () => {
                int prevIndex = (int) prevState;
                int curIndex = (int) curState;

                stateCameras[prevIndex].enabled = false;
                stateCameras[curIndex].enabled = true;
            });
        }
    }
}
