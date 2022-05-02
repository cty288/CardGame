using System.Collections;
using System.Collections.Generic;
using MikroFramework.ActionKit;
using MikroFramework.Architecture;
using UnityEngine;
using Random = System.Random;

namespace MainGame
{
    [ES3Serializable]
    public class AttackMultipleTimesEffect : EffectCommand, IConcatableEffect, IPointable
    {
        [ES3Serializable] private int AttackTime;

        public override bool IsBuffEffect { get; set; } = true;
        public AttackMultipleTimesEffect() : base()
        {

        }

        public AttackMultipleTimesEffect(int attackTime) : base()
        {
            this.AttackTime = attackTime;
        }

        public override EffectCommand OnCloned()
        {
            AttackMultipleTimesEffect cmd = EffectCommand.AllocateEffect<AttackMultipleTimesEffect>();
            cmd.AttackTime = AttackTime;
            return cmd;
        }

        public override MikroAction GetExecuteAnimationEffect()
        {
            return null;
        }

        public override string GetLocalizedTextWithoutBold()
        {
            return Localization("effect_attack_multiple", AttackTime.ToString());
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
            get
            {
                return AttackTime;
            }
        }

        public void OnGenerationPrep()
        {
            Random ran = this.GetSystem<ISeedSystem>().RandomGeneratorRandom;
            AttackTime = ran.Next(2, 6);
        }
    }
}
