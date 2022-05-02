using System.Collections;
using System.Collections.Generic;
using MikroFramework.ActionKit;
using MikroFramework.Architecture;
using UnityEngine;
using Random = System.Random;

namespace MainGame
{
    [ES3Serializable]
    public class AttackMultipleTimesAndKillRandomEnemyEffect : EffectCommand, IConcatableEffect, IPointable
    {
        [ES3Serializable] private int AttackTime;

        public override bool IsBuffEffect { get; set; } = true;
        public AttackMultipleTimesAndKillRandomEnemyEffect() : base()
        {

        }

        public AttackMultipleTimesAndKillRandomEnemyEffect(int attackTime) : base()
        {
            this.AttackTime = attackTime;
        }

        public override EffectCommand OnCloned()
        {
            AttackMultipleTimesAndKillRandomEnemyEffect cmd = EffectCommand.AllocateEffect<AttackMultipleTimesAndKillRandomEnemyEffect>();
            cmd.AttackTime = AttackTime;
            return cmd;
        }

        public override MikroAction GetExecuteAnimationEffect()
        {
            return null;
        }

        public override string GetLocalizedTextWithoutBold()
        {
            return Localization("attack_multiple_time_and_kill_random_enemy", AttackTime.ToString());
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
                return 6 + AttackTime;
            }
        }

        public void OnGenerationPrep()
        {
            Random ran = this.GetSystem<ISeedSystem>().RandomGeneratorRandom;
            AttackTime = ran.Next(1, 4);
        }
    }
}
