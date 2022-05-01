using System.Collections;
using System.Collections.Generic;
using MikroFramework.ActionKit;
using UnityEngine;

namespace MainGame
{
    [ES3Serializable]
    public class NormalAttackCommand : EffectCommand, ITriggeredWhenDealt, IConcatableEffect, IPointable
    {
        public override bool IsBuffEffect { get; set; } = true;
        public NormalAttackCommand() : base()
        {

        }
        public override EffectCommand OnCloned() {
            return EffectCommand.AllocateEffect<NormalAttackCommand>();
        }

        public override MikroAction GetExecuteAnimationEffect() {
            return null;
        }

        public override string GetLocalizedTextWithoutBold() {
            return Localization("eff_attack");
        }

        protected override void OnExecute() {
            
        }

        protected override void Revoke() {
            
        }

        public override void RecycleToCache() {
            EffectCommand.RecycleEffect(this);
        }

        public Rarity Rarity { get; set; } = Rarity.Normal;
        public int CostValue { get; } = 1;
        public void OnGenerationPrep() {
            
        }
    }
}
