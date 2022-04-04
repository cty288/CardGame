using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MainGame
{
    public class TestCharacter : CharacterCardInfo
    {
        public TestCharacter() { }
        public TestCharacter(CardProperties attributes) : base(attributes) {
        }
        
        public override void SetInitialEffects() {
            EffectsProperty = new CardAlterableProperty<List<EffectCommand>>(new List<EffectCommand>() {
                new DealDamageToSelf(10),
                new DealDamageToSelf(20)
            });
        }

        public override void SetInitialPrimitiveBuffEffects() {
            BuffEffectProperty = new CardAlterableProperty<List<EffectCommand>>(new List<EffectCommand>());
        }
    }
}
