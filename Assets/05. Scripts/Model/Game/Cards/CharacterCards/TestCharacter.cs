using System.Collections;
using System.Collections.Generic;
using MikroFramework;
using MikroFramework.ActionKit;
using UnityEngine;

namespace MainGame
{
    [ES3Serializable]
    public class TestCharacter : CharacterCardInfo
    {
        public TestCharacter() { }
        public TestCharacter(CardProperties attributes) : base(attributes) {
        }

        public override List<MikroAction> InitialEffectOverrideAnimation { get; protected set; } =
            new List<MikroAction>() {
                DelayAction.Allocate(2f, () => { Debug.Log("First delay override"); })
            };

        public override List<EffectCommand> SetInitialEffects() {
            return new List<EffectCommand>() {
                new DealDamageToSelf(10)
            };
        }

        public override List<EffectCommand> SetInitialPrimitiveBuffEffects() {
           return new List<EffectCommand>();
        }
    }
}
