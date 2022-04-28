using System.Collections;
using System.Collections.Generic;
using MikroFramework.ActionKit;
using UnityEngine;

namespace MainGame
{
    public class NormalAttackCommand : EffectCommand, ITriggeredWhenDealt, IConcatableEffect
    {
        public override EffectCommand OnCloned() {
            return EffectCommand.AllocateEffect<NormalAttackCommand>();
        }

        public override MikroAction GetExecuteAnimationEffect() {
            return null;
        }

        public override string GetLocalizedTextWithoutBold() {
            return Localization("TestEffect1");
        }

        protected override void OnExecute() {
            
        }

        protected override void Revoke() {
            
        }

        public override void RecycleToCache() {
            EffectCommand.RecycleEffect(this);
        }

        public Rarity Rarity { get; } = Rarity.Normal;
        public int CostValue { get; } = 1;
        public void OnGenerationPrep() {
            
        }
    }
}
