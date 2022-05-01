using System.Collections;
using System.Collections.Generic;
using MikroFramework.ActionKit;
using UnityEngine;

namespace MainGame
{
    [ES3Serializable]
    public class NextCharResurrectionEffect : EffectCommand, ITriggeredWhenDealt, IConcatableEffect {
        public NextCharResurrectionEffect() : base()
        {

        }


        public override bool IsBuffEffect { get; set; } = false;

        public override EffectCommand OnCloned() {
            return EffectCommand.AllocateEffect<NextCharResurrectionEffect>();
        }

        public override MikroAction GetExecuteAnimationEffect() {
            return null;
        }

        public override string GetLocalizedTextWithoutBold() {
            return Localization("eff_resurrection");
        }

        protected override void OnExecute() {
            
        }

        protected override void Revoke() {
           
        }

        public override void RecycleToCache() {
            EffectCommand.RecycleEffect(this);
        }

        public Rarity Rarity { get; set; } = Rarity.Legend;
        public int CostValue { get; } = 8;
        public void OnGenerationPrep() {
            
        }
    }
}
