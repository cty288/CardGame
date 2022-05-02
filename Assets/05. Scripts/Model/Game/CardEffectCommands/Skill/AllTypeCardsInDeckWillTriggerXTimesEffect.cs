using System.Collections;
using System.Collections.Generic;
using MikroFramework.ActionKit;
using MikroFramework.Architecture;
using UnityEngine;

namespace MainGame
{
    [ES3Serializable]
    public class AllTypeCardsWillTriggerXTimesEffect : EffectCommand, IConcatableEffect
    {
        [ES3Serializable] public int TriggerCount;
        [ES3Serializable] public CardType CardType;
        public override bool IsBuffEffect { get; set; } = false;
        public AllTypeCardsWillTriggerXTimesEffect() : base()
        {

        }

        public AllTypeCardsWillTriggerXTimesEffect(CardType cardType, int triggerCount) : base()
        {
            this.TriggerCount = triggerCount;
            this.CardType = cardType;
        }

        public override EffectCommand OnCloned()
        {
            AllTypeCardsWillTriggerXTimesEffect cmd = EffectCommand.AllocateEffect<AllTypeCardsWillTriggerXTimesEffect>();
            cmd.TriggerCount = TriggerCount;
            cmd.CardType = CardType;
            return cmd;
        }

        public override MikroAction GetExecuteAnimationEffect()
        {
            return null;
        }

        public override string GetLocalizedTextWithoutBold()
        {
            return Localization("all_type_card_in_deck_trigger_X_times", 
                CardDisplay.GetCardTypeLocalized(CardType), TriggerCount.ToString());
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
            get {
                return (TriggerCount - 1) * 4;
            }
        }

        public void OnGenerationPrep()
        {
            TriggerCount = this.GetSystem<ISeedSystem>().RandomGeneratorRandom.Next(2, 4);
            CardType = (CardType)this.GetSystem<ISeedSystem>().RandomGeneratorRandom.Next(0,4);
           
        }
    }
}
