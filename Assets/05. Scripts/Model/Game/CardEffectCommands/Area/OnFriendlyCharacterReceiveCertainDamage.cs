using System.Collections;
using System.Collections.Generic;
using MikroFramework.ActionKit;
using MikroFramework.Architecture;
using UnityEngine;

namespace MainGame
{
    [ES3Serializable]
    public class OnFriendlyCharacterReceiveCertainDamage : AbstractConcatableEffectTriggerdByEventWhenDealt{
        public override bool IsBuffEffect { get; set; } = false;
        [ES3Serializable] private int DamageAmount;
        public OnFriendlyCharacterReceiveCertainDamage() : base()
        {

        }
        public OnFriendlyCharacterReceiveCertainDamage(int damageAmount, params EffectCommand[] effectTriggered) : base(effectTriggered) {
            this.DamageAmount = damageAmount;
        }

        public override MikroAction GetExecuteAnimationEffect()
        {
            return null;
        }

        protected override void Revoke()
        {

        }

        public override void RecycleToCache()
        {
            EffectCommand.RecycleEffect(this);
        }

        public override List<IBattleEvent> TriggeredBy { get; set; }


        [ES3Serializable]
        public override List<EffectCommand> TriggeredEffects { get; set; }
        public override ITriggeredByEventsWhenDealt OnClone()
        {
            OnFriendlyCharacterReceiveCertainDamage cmd = EffectCommand.AllocateEffect<OnFriendlyCharacterReceiveCertainDamage>();
            cmd.DamageAmount = DamageAmount;
            return cmd;
        }

        public override string GetConditionLocalizedPrefixTextWithoutBold()
        {
            return Localization("effect_conditional_on_friendly_receive_certain_damage", DamageAmount.ToString());
        }

        public override void OnGenerationPrep() {
            base.OnGenerationPrep();
            DamageAmount = this.GetSystem<ISeedSystem>().RandomGeneratorRandom.Next(5, 30);
        }

        public override float ConditionBaseCost {
            get {
                return (1 - (DamageAmount - 5) / 25f) * 5;
            }

            protected set {}
        }
    }
}
