using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework;
using MikroFramework.ActionKit;
using MikroFramework.Architecture;
using MikroFramework.Pool;
using UnityEngine;

namespace MainGame
{
    [ES3Serializable]
    public class DealDamageToSelf : EffectCommand, ITriggeredWhenDealt, IConcatableEffect {
        [ES3Serializable] public int DamageCount;

        public DealDamageToSelf() : base() {
            
        }

        public DealDamageToSelf(int damage) : base() {
            DamageCount = damage;
        }


        public override EffectCommand OnCloned() {
            DealDamageToSelf cmd = SafeObjectPool<DealDamageToSelf>.Singleton.Allocate();
            cmd.DamageCount = this.DamageCount;
            return cmd;
        }

        public override MikroAction GetExecuteAnimationEffect() {
           // Debug.Log(GetCardDisplayBelongTo().gameObject.name);
            return DelayAction.Allocate(3f, () => { Debug.Log($"Deal damage finished, {GetCardDisplayBelongTo().gameObject.name}"); });
        }

        public override string GetLocalizedTextWithoutBold() {
            return Localization("eff_deal_self_damage", DamageCount.ToString());
        }

        protected override void OnExecute() {
            ((CharacterCardInfo) CardBelongTo).Health.CurrentValue.Value -= 10;
            Debug.Log("Dealt");
        }

        protected override void Revoke() {
            
        }

        public override void RecycleToCache() {
            SafeObjectPool<DealDamageToSelf>.Singleton.Recycle(this);
        }

        [ES3Serializable] public Rarity Rarity { get;  set; } = Rarity.Normal;


        public int CostValue {
            get {
                return Mathf.CeilToInt(DamageCount / 5f);
            }
        }

       
        public void OnGenerationPrep() {
            DamageCount = this.GetSystem<ISeedSystem>().RandomGeneratorRandom.Next(-20, 0);
        }
    }
}
