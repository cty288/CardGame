using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

namespace MainGame
{
    public class TryDealCardCommand : AbstractCommand<TryDealCardCommand>
    {
        private CardInfo cardInfo;
        public TryDealCardCommand() { }

        public TryDealCardCommand(CardInfo cardInfo)
        {
            this.cardInfo = cardInfo;
        }

        protected override void OnExecute()
        {
            OnCardTryDealt e = new OnCardTryDealt() { DealSuccess = true, CardInfo = cardInfo };
            this.SendEvent<OnCardTryDealt>(e);
            if (e.DealSuccess)
            {
                Debug.Log("Deal Success");
                cardInfo.OnCardDealtSuccess();
                this.SendEvent<IBattleEvent>(new OnCardDealt() { CardDealt = cardInfo });
            }
        }
    }
}