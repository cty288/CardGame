using System.Collections;
using System.Collections.Generic;
using MikroFramework.ActionKit;
using MikroFramework.Architecture;
using UnityEngine;

namespace MainGame
{
    [ES3Serializable]
    public class DrawCardsEffect : EffectCommand, ITriggeredWhenDealt, IConcatableEffect {
        public override bool IsBuffEffect { get; set; } = false;
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
                return Mathf.CeilToInt(numCardsDraw * 1.5f);
            }
        }

        public void OnGenerationPrep() {
            int chance = this.GetSystem<ISeedSystem>().RandomGeneratorRandom.Next(0, 100);
            if (chance <= 50) {
                numCardsDraw = this.GetSystem<ISeedSystem>().RandomGeneratorRandom.Next(1, 4);
            }else if (chance <= 70) {
                numCardsDraw = this.GetSystem<ISeedSystem>().RandomGeneratorRandom.Next(1, 5);
            }else
            {
                numCardsDraw = this.GetSystem<ISeedSystem>().RandomGeneratorRandom.Next(1, 6);
            }
            
        }
    }
}
