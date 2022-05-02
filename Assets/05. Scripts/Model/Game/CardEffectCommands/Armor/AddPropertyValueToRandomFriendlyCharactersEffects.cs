using System.Collections;
using System.Collections.Generic;
using MikroFramework.ActionKit;
using MikroFramework.Architecture;
using UnityEngine;
using Random = System.Random;

namespace MainGame
{
    [ES3Serializable]
    public class AddPropertyValueToRandomFriendlyCharactersEffects : EffectCommand, IConcatableEffect
    {
        [ES3Serializable] private int CharacterCount;
        [ES3Serializable] private int AttackAdded;
        [ES3Serializable] private int HealthAdded;
        public override bool IsBuffEffect { get; set; } = false;
        public AddPropertyValueToRandomFriendlyCharactersEffects() : base()
        {

        }

        public AddPropertyValueToRandomFriendlyCharactersEffects(int AttackAdded, int HealthAdded, int characterCount) : base()
        {
            this.CharacterCount = characterCount;
            this.AttackAdded = AttackAdded;
            this.HealthAdded = HealthAdded;
        }

        public override EffectCommand OnCloned()
        {
            AddPropertyValueToRandomFriendlyCharactersEffects cmd = EffectCommand.AllocateEffect<AddPropertyValueToRandomFriendlyCharactersEffects>();
            cmd.CharacterCount = CharacterCount;
            cmd.AttackAdded = AttackAdded;
            cmd.HealthAdded = HealthAdded;
            return cmd;
        }

        public override MikroAction GetExecuteAnimationEffect()
        {
            return null;
        }

        public override string GetLocalizedTextWithoutBold()
        {
            return Localization("effect_add_properties_to_random_characters", CharacterCount.ToString(),
                AttackAdded.ToString(), HealthAdded.ToString());
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

        public Rarity Rarity { get; set; } = Rarity.Rare;

        public int CostValue
        {
            get {
                return Mathf.Min(((AttackAdded + HealthAdded) - 2) / 2 * CharacterCount, 10);
            }
        }

        public void OnGenerationPrep()
        {
            Random ran = this.GetSystem<ISeedSystem>().RandomGeneratorRandom;
            CharacterCount = ran.Next(1, 4);
            AttackAdded = ran.Next(1, 6);
            HealthAdded = ran.Next(0, 6);
        }
    }
}
