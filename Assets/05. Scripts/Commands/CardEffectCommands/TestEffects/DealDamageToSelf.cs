using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework;
using MikroFramework.ActionKit;
using MikroFramework.Pool;
using UnityEngine;

namespace MainGame
{
    [ES3Serializable]
    public class DealDamageToSelf : EffectCommand, ITriggeredWhenDealt {
        [ES3Serializable] public int DamageCount;

        public DealDamageToSelf() : base() {
            
        }

        public DealDamageToSelf(int damage) : base() {
            DamageCount = damage;
        }


        public override EffectCommand OnCloned() {
            return this;
        }

        public override MikroAction GetExecuteAnimationEffect() {
           // Debug.Log(GetCardDisplayBelongTo().gameObject.name);
            return DelayAction.Allocate(3f, () => { Debug.Log($"Deal damage finished, {GetCardDisplayBelongTo().gameObject.name}"); });
        }

        public override string GetLocalizedTextWithoutBold() {
            return Localization("TestEffect1", DamageCount.ToString());
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
    }
}
