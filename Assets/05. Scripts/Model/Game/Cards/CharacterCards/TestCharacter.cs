using System.Collections;
using System.Collections.Generic;
using MikroFramework;
using MikroFramework.ActionKit;
using UnityEngine;

namespace MainGame
{
    public class TestCharacter : CharacterCardInfo
    {
        public TestCharacter() { }
        public TestCharacter(CardProperties attributes) : base(attributes) {
        }

        public override List<MikroAction> InitialEffectOverrideAnimation { get; protected set; } =
            new List<MikroAction>() {
                DelayAction.Allocate(2f, () => { Debug.Log("First delay override"); })
            };

        public override void SetInitialEffects() {
            EffectsProperty = new CardAlterableProperty<List<EffectCommand>>(new List<EffectCommand>() {
                new DealDamageToSelf(10)
            });
        }

        public override void SetInitialPrimitiveBuffEffects() {
            BuffEffectProperty = new CardAlterableProperty<List<EffectCommand>>(new List<EffectCommand>());
        }
    }
}
