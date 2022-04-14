using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MainGame
{
    [RequireComponent(typeof(CardDisplay))]
    public class CardDrag : AbstractMikroController<CardGame>, IDragHandler, IEndDragHandler, IBeginDragHandler {
        private CardDisplay card;

        public bool CanUse {
            get {
                return GetComponent<Trigger2DCheck>().Triggered;
            }
        }
        private bool inHand = true;
        private int lastPositionInHand = -1;
        private Transform previousParent;

        private void Awake() {
            card = GetComponent<CardDisplay>();
        }


        public void OnDrag(PointerEventData eventData) {
            transform.position = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData) {
            if (!inHand) {
                //need more checks
                if (!CanUse) {
                    inHand = true;
                    transform.parent = previousParent;
                    transform.SetSiblingIndex(lastPositionInHand);
                }
                else {
                    this.SendCommand<DragAndDealCardCommand>(new DragAndDealCardCommand(card.CardInfo.Value));
                   // Destroy(this.gameObject);
                   this.GetComponent<CardDisplay>().RecycleToCache();
                }
            }
        }

        public void OnBeginDrag(PointerEventData eventData) {
            if (inHand) {
                lastPositionInHand = transform.GetSiblingIndex();
                previousParent = transform.parent;
                transform.parent = transform.parent.parent;
                inHand = false;
            }
        }
    }
}
