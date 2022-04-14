using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework;
using MikroFramework.ActionKit;
using MikroFramework.Architecture;
using MikroFramework.Event;
using MikroFramework.Pool;
using UnityEngine;

namespace MainGame
{
    public class HandCardContainerViewController : AbstractMikroController<CardGame> {
        [SerializeField] private GameObject characterHandCardPrefab;
        [SerializeField] private Transform characterHandCardSpawnPosition;
        private IBattleEventControlSystem eventControlSystem;
        private SafeGameObjectPool characterHandCardPool;
        private void Awake() {
            characterHandCardPool = GameObjectPoolManager.Singleton.CreatePool(characterHandCardPrefab, 10, 15);
            eventControlSystem = this.GetSystem<IBattleEventControlSystem>();

            this.GetModel<IGameStateModel>().GameState.RegisterWithInitValue(CheckBattleState)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
          
        }

        private void CheckBattleState(GameState e) {
            if (e == GameState.Battle) {
                RegisterBattleAnimationEvents();
            }
        }

        private void RegisterBattleAnimationEvents() {
            eventControlSystem.RegisterEffectToBattleEvent(typeof(OnDrawCard), OnEnterBattleSceneDrawCards).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void OnEnterBattleSceneDrawCards(IBattleEvent e) {
           
            Sequence drawCardAnimationSequence = Sequence.Allocate();
            Debug.Log("OnEnterBattleSceneDrawCards Creating Sequence");

            drawCardAnimationSequence.AddAction(CallbackAction.Allocate(() => {
                Debug.Log("OnEnterBattleSceneDrawCards");
                GameObject characterHandCard = characterHandCardPool.Allocate();
                characterHandCard.GetComponent<CardDisplay>().CardInfo.Value = ((OnDrawCard) e).cardDraw;
                characterHandCard.GetComponent<CardDisplay>().CardInfo.Value.CardDisplayBelongTo =
                    characterHandCard.GetComponent<CardDisplay>();
                Debug.Log(characterHandCard.GetComponent<CardDisplay>().CardDisplayInfo.CardIllustration.name);
                characterHandCard.gameObject.transform.position = characterHandCardSpawnPosition.position;
                characterHandCard.transform.SetParent(transform);
                characterHandCard.transform.localScale = new Vector3(1, 1, 1);
            })).AddAction(DelayAction.Allocate(0.2f));

            eventControlSystem.RegisterAnimationToSequence(drawCardAnimationSequence);
        }
    }
}
