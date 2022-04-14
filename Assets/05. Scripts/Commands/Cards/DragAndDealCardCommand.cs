using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

namespace MainGame
{
    public class DragAndDealCardCommand : AbstractCommand<DragAndDealCardCommand> {
        private CardInfo cardInfo;
        public DragAndDealCardCommand(){}

        public DragAndDealCardCommand(CardInfo cardInfo) {
            this.cardInfo = cardInfo;
        }
        protected override void OnExecute() {
            cardInfo.TryDealCard();
        }
    }
}
