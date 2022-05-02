using System.Collections;
using System.Collections.Generic;
using MikroFramework.ActionKit;
using MikroFramework.Architecture;
using UnityEngine;
using Random = System.Random;

namespace MainGame
{
    [ES3Serializable]
    public class DealDamageToAllEnemiesWhenAttackEffect  : EffectCommand, IConcatableEffect, IPointable
    {
        [ES3Serializable] private int Damage;
     
        public override bool IsBuffEffect { get; set; } = false;
        public DealDamageToAllEnemiesWhenAttackEffect() : base()
        {

        }

        public DealDamageToAllEnemiesWhenAttackEffect(int damage) : base()
        {
            this.Damage = damage;
        }

        public override EffectCommand OnCloned()
        {
            DealDamageToAllEnemiesWhenAttackEffect cmd = EffectCommand.AllocateEffect<DealDamageToAllEnemiesWhenAttackEffect>();
            cmd.Damage = Damage;
            return cmd;
        }

        public override MikroAction GetExecuteAnimationEffect()
        {
            return null;
        }

        public override string GetLocalizedTextWithoutBold()
        {
            return Localization("effect_deal_damage_to_all_enemies_when_attack", Damage.ToString());
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
                return Damage;
            }
        }

        public void OnGenerationPrep()
        {
            Random ran = this.GetSystem<ISeedSystem>().RandomGeneratorRandom;
            Damage = ran.Next(1, 10);
        }
    }
}
