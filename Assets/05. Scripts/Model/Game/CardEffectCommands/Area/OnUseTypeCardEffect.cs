using System.Collections;
using System.Collections.Generic;
using MikroFramework.ActionKit;
using MikroFramework.Architecture;
using UnityEngine;

namespace MainGame
{
    [ES3Serializable]
    public class OnUseTypeCardEffect : AbstractConcatableEffectTriggerdByEventWhenDealt {
        [ES3Serializable]
        private CardType cardType;

        public override bool IsBuffEffect { get; set; } = false;
        public OnUseTypeCardEffect() : base()
        {

        }

        public OnUseTypeCardEffect(params EffectCommand[] effectTriggered) : base(effectTriggered) {
        }


        public OnUseTypeCardEffect(CardType cardType, params EffectCommand[] effectTriggered) : base(effectTriggered) {
            this.cardType = cardType;
        }

        public override MikroAction GetExecuteAnimationEffect() {
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
            OnUseTypeCardEffect cmd = EffectCommand.AllocateEffect<OnUseTypeCardEffect>();
            cmd.cardType = cardType;
            return cmd;
        }

        public override string GetConditionLocalizedPrefixTextWithoutBold() {
            return Localization("effect_conditional_on_use_type_card", CardDisplay.GetCardTypeLocalized(cardType));
        }

        public override void OnGenerationPrep() {
            base.OnGenerationPrep();
            cardType =(CardType) this.GetSystem<ISeedSystem>().RandomGeneratorRandom.Next(0, 5);
        }

        public override float ConditionBaseCost { get; protected set; } = 3;
    }
}
