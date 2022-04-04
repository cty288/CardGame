using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Pool;
using UnityEngine;

namespace MainGame
{
    [ES3Serializable]
    public class DealDamageToSelf : EffectCommand {
        [ES3Serializable] public int DamageCount;

        public DealDamageToSelf() : base() {
            
        }

        public DealDamageToSelf(int damage) : base() {
            DamageCount = damage;
        }


        public override EffectCommand OnCloned() {
            return this;
        }

        public override EffectBattleEventTriggerType BattleEventTriggerType { get; protected set; } =
            EffectBattleEventTriggerType.TriggerWhenDealt;

        public override Type BattleEventTypeRegister { get; protected set; } = null;
        public override string GetLocalizedTextWithoutBold() {
            return Localization("TestEffect1", DamageCount.ToString());
        }

        protected override void OnExecute() {
            ((CharacterCardInfo) CardBelongTo).Health.CurrentValue.Value -= 10;
        }

        protected override void Revoke() {
            
        }

        public override void RecycleToCache() {
            SafeObjectPool<DealDamageToSelf>.Singleton.Recycle(this);
        }
    }
}
