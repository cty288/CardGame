using System.Collections;
using System.Collections.Generic;
using MikroFramework.ActionKit;
using MikroFramework.Architecture;
using UnityEngine;
using Random = System.Random;

namespace MainGame
{
    [ES3Serializable]
    public class AddPropertyValueEffect : EffectCommand, ITriggeredWhenDealt, IConcatableEffect {
        [ES3Serializable] public Vector2Int AddedValues;
        public AddPropertyValueEffect() : base()
        {

        }

        public AddPropertyValueEffect(Vector2Int addedValues) : base() {
            this.AddedValues = addedValues;
        }

        public override EffectCommand OnCloned() {
            AddPropertyValueEffect cmd = EffectCommand.AllocateEffect<AddPropertyValueEffect>();
            cmd.AddedValues = AddedValues;
            return cmd;
        }

        public override MikroAction GetExecuteAnimationEffect() {
            return null;
        }

        public override string GetLocalizedTextWithoutBold() {
            string sign1 = AddedValues.x < 0 ? "-" : "+";
            string sign2 = AddedValues.y < 0 ? "-" : "+";
            return Polyglot.Localization.GetFormat("eff_add_property", sign1, AddedValues.x, sign2, AddedValues.y);
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
                return ((AddedValues.x + AddedValues.y) - 2) / 2;
            }
        }

        public void OnGenerationPrep() {
            Random ran = this.GetSystem<ISeedSystem>().RandomGeneratorRandom;
            AddedValues = new Vector2Int(ran.Next(0, 11), ran.Next(0, 11));
        }
    }
}
