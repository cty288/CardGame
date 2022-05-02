using System.Collections;
using System.Collections.Generic;
using MikroFramework.ActionKit;
using MikroFramework.Architecture;
using UnityEngine;
using Random = System.Random;

namespace MainGame
{
    [ES3Serializable]
    public class LetRandomFriendlyCharactersAttackRandomlyEffect : EffectCommand, IConcatableEffect
    {
        [ES3Serializable] private int CharacterCount;

        public override bool IsBuffEffect { get; set; } = false;
        public LetRandomFriendlyCharactersAttackRandomlyEffect() : base()
        {

        }

        public LetRandomFriendlyCharactersAttackRandomlyEffect(int characterCount) : base()
        {
            this.CharacterCount = characterCount;
        }

        public override EffectCommand OnCloned()
        {
            LetRandomFriendlyCharactersAttackRandomlyEffect cmd = EffectCommand.AllocateEffect<LetRandomFriendlyCharactersAttackRandomlyEffect>();
            cmd.CharacterCount = CharacterCount;
            return cmd;
        }

        public override MikroAction GetExecuteAnimationEffect()
        {
            return null;
        }

        public override string GetLocalizedTextWithoutBold()
        {
            return Localization("random_char_attack_random_enemies", CharacterCount.ToString());
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
            get
            {
                return Mathf.Max(CharacterCount-1,0);
            }
        }

        public void OnGenerationPrep()
        {
            Random ran = this.GetSystem<ISeedSystem>().RandomGeneratorRandom;
            CharacterCount = ran.Next(2, 5);
        }
    }
}
