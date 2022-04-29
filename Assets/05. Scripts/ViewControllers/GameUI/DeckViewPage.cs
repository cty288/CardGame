using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using MikroFramework;
using TMPro;
using MikroFramework.ActionKit;
using System.Collections.Generic;
using System.Linq;
using MikroFramework.Architecture;
using MikroFramework.Pool;

namespace MainGame {
	public partial class DeckViewPage : AbstractMikroController<CardGame> {
        [SerializeField] private GameObject cardPrefab;
        private SafeGameObjectPool pool;

        private List<CardDisplay> cardCurrentDisplay = new List<CardDisplay>();
        private void Awake() {
           pool = GameObjectPoolManager.Singleton.GetOrCreatePool(cardPrefab);
        }

        private void OnEnable() {
            List<CardInfo> cards = this.GetModel<IGameCardDeckModel>().CharactersInDeck.Value
                .Select(info => info as CardInfo).ToList();
            Debug.Log(this.GetModel<IGameCardDeckModel>().CardsInDeck.Value.Count);
            cards.AddRange(this.GetModel<IGameCardDeckModel>().CardsInDeck.Value);

            foreach (CardInfo cardInfo in cards) {
                CardDisplay card = pool.Allocate().GetComponent<CardDisplay>();
                card.CardInfo.Value = cardInfo;
                card.RefreshCardDisplay();
                card.transform.SetParent(ObjViewLayout.transform);
                cardCurrentDisplay.Add(card);
            }
        }

        public void ClosePage() {
            foreach (CardDisplay cardDisplay in cardCurrentDisplay)
            {
                pool.Recycle(cardDisplay.gameObject);
            }
            cardCurrentDisplay.Clear();
        }
        private void OnDisable() {
         
        }
    }
}