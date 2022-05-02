using System.Collections;
using System.Collections.Generic;
using MikroFramework.ActionKit;
using MikroFramework.Architecture;
using UnityEngine;

namespace MainGame
{
    [ES3Serializable]
    public class ReduceCostInHandsEffect :  EffectCommand, IConcatableEffect{
        [ES3Serializable] public int ReducedCost;
       
        public override bool IsBuffEffect { get; set; } = false;
        public ReduceCostInHandsEffect() : base()
        {

        }

        public ReduceCostInHandsEffect(int reducedCost) : base() {
            this.ReducedCost = reducedCost;
        }

        public override EffectCommand OnCloned()
        {
            ReduceCostInHandsEffect cmd = EffectCommand.AllocateEffect<ReduceCostInHandsEffect>();
            cmd.ReducedCost = ReducedCost;
            return cmd;
        }

        public override MikroAction GetExecuteAnimationEffect()
        {
            return null;
        }

        public override string GetLocalizedTextWithoutBold()
        {
            return Localization("effect_hand_card_cost_decrease", ReducedCost.ToString());
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

        public Rarity Rarity { get; set; } = Rarity.Epic;

        public int CostValue
        {
            get {
                return Mathf.CeilToInt( ReducedCost * 2.5f);
            }
        }

        public void OnGenerationPrep() {
            ReducedCost = this.GetSystem<ISeedSystem>().RandomGeneratorRandom.Next(1, 5);
        }
    }
}
