using System.Collections;
using System.Collections.Generic;
using MikroFramework.ActionKit;
using MikroFramework.Architecture;
using UnityEngine;

namespace MainGame
{
    [ES3Serializable]
    public class DealDamage2RandomEnemy : EffectCommand, IConcatableEffect{
        [ES3Serializable] public int Damage;
        public DealDamage2RandomEnemy() : base()
        {

        }

        public DealDamage2RandomEnemy(int damage) : base() {
            this.Damage = damage;
        }

        public override EffectCommand OnCloned() {
            DealDamage2RandomEnemy cmd = EffectCommand.AllocateEffect<DealDamage2RandomEnemy>();
            cmd.Damage = Damage;
            return cmd;
        }

        public override MikroAction GetExecuteAnimationEffect() {
            return null;
        }

        public override string GetLocalizedTextWithoutBold() {
            return Localization("eff_deal_damage_to_random_enemy", Damage.ToString());
        }

        protected override void OnExecute() {
           
        }

        protected override void Revoke() {
            
        }

        public override void RecycleToCache() {
            EffectCommand.RecycleEffect(this);
        }

        public Rarity Rarity { get; set; } = Rarity.Normal;

        public int CostValue {
            get {
                return Damage / 3;
            }
        }

        public void OnGenerationPrep() {
            Damage = this.GetSystem<ISeedSystem>().RandomGeneratorRandom.Next(3, 20);
        }
    }
}
