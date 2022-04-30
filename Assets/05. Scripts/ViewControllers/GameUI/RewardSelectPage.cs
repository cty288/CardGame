using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using MikroFramework;
using TMPro;
using MikroFramework.ActionKit;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Event;
using MikroFramework.Pool;

namespace MainGame {
	public partial class RewardSelectPage : AbstractMikroController<CardGame> {
        [SerializeField] 
        private GameObject cardDisplayPrefab;

        private List<CardDisplay> rewardsThisTime = new List<CardDisplay>();

        [SerializeField]
        private SafeGameObjectPool rewardCardDisplayPool;

        private bool cardClicked = false;
        private void Awake() {
            this.RegisterEvent<OnCardRewardGenerated>(OnRewardGenerated).UnRegisterWhenGameObjectDestroyed(gameObject);
            rewardCardDisplayPool = GameObjectPoolManager.Singleton.GetOrCreatePool(cardDisplayPrefab);
        }

        private void OnRewardGenerated(OnCardRewardGenerated e) {
            cardClicked = false;
            this.GetSystem<IMapAnimationControlSystem>().AddUnblockableAsyncAnimation(UntilAction.Allocate(() => cardClicked));
            ObjPage.gameObject.SetActive(true);
            foreach (CardInfo cardInfo in e.RewardCard) {
                CardDisplay generated = rewardCardDisplayPool.Allocate().GetComponent<CardDisplay>();
                generated.gameObject.GetComponent<PoolableGameObject>().Pool = rewardCardDisplayPool;
                generated.CardInfo.Value = cardInfo;
                generated.transform.SetParent(ObjViewLayout.transform);
                generated.RefreshCardDisplay();
                generated.OnClicked += OnCardClicked;
                rewardsThisTime.Add(generated);
            }
        }

        private void OnCardClicked(CardDisplay card) {
            this.SendCommand(AddCardToPermantDeck.Allocate(card.CardInfo.Value));
            cardClicked = true;
            foreach (CardDisplay cardDisplay in rewardsThisTime) {
                cardDisplay.OnClicked -= OnCardClicked;
                cardDisplay.transform.SetParent(cardDisplay.GetComponent<PoolableGameObject>().Pool
                    .transform);
                rewardCardDisplayPool.Recycle(cardDisplay.gameObject);
            }
            rewardsThisTime.Clear();
            ObjPage.gameObject.SetActive(false);
        }
    }
}