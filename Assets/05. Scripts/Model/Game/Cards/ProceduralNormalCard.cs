using System.Collections;
using System.Collections.Generic;
using MikroFramework.ActionKit;
using MikroFramework.Architecture;
using UnityEngine;

namespace MainGame
{
    [ES3Serializable]
    public class ProceduralNormalCard : CardInfo {
        
        public ProceduralNormalCard() { }
        public ProceduralNormalCard(int cost, Rarity rarity, CardType cardType, string nameKey,
            List<EffectCommand> effects) {
            CostProperty = new CardAlterableProperty<int>(cost, i => i);
            CardRarity = rarity;
            CardType = cardType;
            NameKey = nameKey;
            this.RegisterEvent<IBattleEvent>(OnEnterBattleScene);
            this.RegisterEvent<IBattleEvent>(OnLeaveBattleScene);
            // this.RegisterEvent<OnEnterBattleScene>(OnEnterBattleScene);
            // this.RegisterEvent<OnLeaveBattleScene>(OnLeaveBattleScene);

            EffectsProperty = new CardAlterableProperty<List<EffectCommand>>(new List<EffectCommand>(), list => new List<EffectCommand>());
            BuffEffectProperty = new CardAlterableProperty<List<EffectCommand>>(new List<EffectCommand>(), list => new List<EffectCommand>() );

            foreach (EffectCommand effect in effects) {
                if (effect.IsBuffEffect) {
                    BuffEffectProperty.PrimitiveValue.Add(effect.Clone());
                    BuffEffectProperty.CurrentValue.Value.Add(effect.Clone());
                    effect.RecycleToCache();
                }
                else {
                    EffectsProperty.PrimitiveValue.Add(effect.Clone());
                    EffectsProperty.CurrentValue.Value.Add(effect.Clone());
                   effect.RecycleToCache();
                }
            }
            SetEffectsToPrimitive();
        }
        public override List<MikroAction> InitialEffectOverrideAnimation { get; protected set; }
        public override List<EffectCommand> SetInitialEffects() {

            return new List<EffectCommand>();
        }

        public override List<EffectCommand> SetInitialPrimitiveBuffEffects() {
            return new List<EffectCommand>();
        }

        public override void OnCardDealtSuccess() {
            
        }
    }
}
