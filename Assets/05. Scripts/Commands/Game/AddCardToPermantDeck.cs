using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Pool;
using UnityEngine;

namespace MainGame
{
    /// <summary>
    /// Add to game card deck model, not the battle system's deck
    /// </summary>
    public class AddCardToPermantDeck : AbstractCommand<AddCardToPermantDeck> {
        private CardInfo cardInfo;

        public static AddCardToPermantDeck Allocate(CardInfo card) {
            AddCardToPermantDeck cmd = SafeObjectPool<AddCardToPermantDeck>.Singleton.Allocate();
            cmd.cardInfo = card;
            return cmd;
        }
        protected override void OnExecute() {
            if (cardInfo.CardType == CardType.Character) {
                this.GetModel<IGameCardDeckModel>().CharactersInDeck.Value.Add(cardInfo as CharacterCardInfo);
            }else {
                this.GetModel<IGameCardDeckModel>().CardsInDeck.Value.Add(cardInfo);
            }
        }
    }
}
