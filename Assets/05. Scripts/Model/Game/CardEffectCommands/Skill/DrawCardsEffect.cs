using System.Collections;
using System.Collections.Generic;
using MikroFramework.ActionKit;
using UnityEngine;

namespace MainGame
{
    [ES3Serializable]
    public class DrawCardsEffect : EffectCommand, ITriggeredWhenDealt, IConcatableEffect {
        [ES3Serializable]
        public int numCardsDraw = 1; 
        public DrawCardsEffect() : base() {

        }

        public DrawCardsEffect(int numDraw) : base() {
            this.numCardsDraw = numDraw;
        }
        public override EffectCommand OnCloned() {
            DrawCardsEffect cmd = EffectCommand.AllocateEffect<DrawCardsEffect>();
            cmd.numCardsDraw = numCardsDraw;
            return cmd;
        }

        public override MikroAction GetExecuteAnimationEffect() {
            return null;
        }

        public override string GetLocalizedTextWithoutBold() {
            string surfix = numCardsDraw > 1 ? "s" : "";
            return Polyglot.Localization.GetFormat("eff_draw_cards", numCardsDraw, surfix);
        }

        protected override void OnExecute() {
            
        }

        protected override void Revoke() {
            
        }

        public override void RecycleToCache() {
           EffectCommand.RecycleEffect(this);
        }

        public Rarity Rarity { get; set; } = Rarity.Normal;

        public int CostValue {
            get {
                return Mathf.FloorToInt(numCardsDraw * 1.5f);
            }
        }

        public void OnGenerationPrep() {
          
        }
    }
}
