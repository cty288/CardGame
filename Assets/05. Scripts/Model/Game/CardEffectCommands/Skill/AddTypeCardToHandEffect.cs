using System.Collections;
using System.Collections.Generic;
using MikroFramework.ActionKit;
using MikroFramework.Architecture;
using UnityEngine;

namespace MainGame
{
    [ES3Serializable]
    public class AddTypeCardToHandEffect : EffectCommand, IConcatableEffect
    {
        [ES3Serializable] public int CardCount;
        [ES3Serializable] public CardType CardType;
        public override bool IsBuffEffect { get; set; } = false;
        public AddTypeCardToHandEffect() : base()
        {

        }

        public AddTypeCardToHandEffect(CardType cardType, int cardCount) : base() {
            this.CardCount = cardCount;
            this.CardType = cardType;
        }

        public override EffectCommand OnCloned()
        {
            AddTypeCardToHandEffect cmd = EffectCommand.AllocateEffect<AddTypeCardToHandEffect>();
            cmd.CardCount = CardCount;
            cmd.CardType = CardType;
            return cmd;
        }

        public override MikroAction GetExecuteAnimationEffect()
        {
            return null;
        }

        public override string GetLocalizedTextWithoutBold() {
            return Localization("effect_add_type_cards_to_hand", CardCount.ToString(),
                CardDisplay.GetCardTypeLocalized(CardType));
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

        public Rarity Rarity { get; set; } = Rarity.Rare;

        public int CostValue
        {
            get {
                return (Mathf.CeilToInt(CardCount * 1.25f));
            }
        }

        public void OnGenerationPrep()
        {
            CardCount = this.GetSystem<ISeedSystem>().RandomGeneratorRandom.Next(1, 6);
            CardType =(CardType)this.GetSystem<ISeedSystem>().RandomGeneratorRandom.Next(0, 4);
        }
    }
}
