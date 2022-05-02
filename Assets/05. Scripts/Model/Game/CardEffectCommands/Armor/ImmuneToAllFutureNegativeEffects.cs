using System.Collections;
using System.Collections.Generic;
using MikroFramework.ActionKit;
using MikroFramework.Architecture;
using UnityEngine;

namespace MainGame
{
    [ES3Serializable]
    public class ImmuneToAllFutureNegativeEffects : EffectCommand, IConcatableEffect
    {
        
        public override bool IsBuffEffect { get; set; } = false;
        public ImmuneToAllFutureNegativeEffects() : base()
        {

        }


        public override EffectCommand OnCloned()
        {
            ImmuneToAllFutureNegativeEffects cmd = EffectCommand.AllocateEffect<ImmuneToAllFutureNegativeEffects>();
            return cmd;
        }

        public override MikroAction GetExecuteAnimationEffect()
        {
            return null;
        }

        public override string GetLocalizedTextWithoutBold()
        {
            return Localization("effect_immune_to_all_future_negative_effects");
        }

        protected override void OnExecute()
        {

        }

        protected override void Revoke()
        {

        }

        public override void RecycleToCache()
        {
            EffectCommand.RecycleEffect(this);
        }

        public Rarity Rarity { get; set; } = Rarity.Legend;

        public int CostValue
        {
            get
            {
                return 6;
            }
        }

        public void OnGenerationPrep() {
            
        }
    }
}
