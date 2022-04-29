using System.Collections;
using System.Collections.Generic;
using MikroFramework.ActionKit;
using UnityEngine;

namespace MainGame
{
  
    [ES3Serializable]
    public class NormalAttack : CardInfo {
        
        public NormalAttack() { }
        public NormalAttack(CardProperties attributes) : base(attributes)
        {
        }

        public override List<MikroAction> InitialEffectOverrideAnimation { get; protected set; }
        public override List<EffectCommand> SetInitialEffects() {
            return new List<EffectCommand>() {
               
            };
        }

        public override List<EffectCommand> SetInitialPrimitiveBuffEffects() {
            return new List<EffectCommand>() {
                new NormalAttackCommand()
            };
        }

        public override void OnCardDealtSuccess() {
            
        }
    }
}
