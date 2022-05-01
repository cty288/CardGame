using System.Collections;
using System.Collections.Generic;
using MikroFramework.ActionKit;
using UnityEngine;

namespace MainGame
{
    [ES3Serializable]
    public class OnFriendlyCharacterHurtEffect : AbstractConcatableEffectTriggerdByEventWhenDealt
    {
        public override bool IsBuffEffect { get; set; } = false;
        public OnFriendlyCharacterHurtEffect() : base() {

        }
        public OnFriendlyCharacterHurtEffect(params EffectCommand[] effectTriggered) : base() {
            TriggeredEffects = new List<EffectCommand>();
            TriggeredEffects.AddRange(effectTriggered);
        }

        public override MikroAction GetExecuteAnimationEffect() {
            return null;
        }

        protected override void Revoke() {
            
        }

        public override void RecycleToCache() {
            EffectCommand.RecycleEffect(this);
        }

        public override List<IBattleEvent> TriggeredBy { get; set; }


        [ES3Serializable]
        public override List<EffectCommand> TriggeredEffects { get; set; }
        public override ITriggeredByEventsWhenDealt OnClone() {
            return EffectCommand.AllocateEffect<OnFriendlyCharacterHurtEffect>();
        }

        public override string GetConditionLocalizedPrefixTextWithoutBold() {
            return Localization("eff_conditional_on_friendly_hurt");
        }

        public override float ConditionBaseCost { get; protected set; } = 2;
    }
}
