using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MikroFramework.Architecture;
using MikroFramework.DataStructures;
using MikroFramework.Pool;
using UnityEngine;

namespace MainGame
{
    public class EffectTable : Table<EffectCommand> 
    {
        public TableIndex<Rarity, IConcatableEffect> ConcatableRarityIndex { get; private set; }

        public EffectTable() {
            ConcatableRarityIndex = new TableIndex<Rarity, IConcatableEffect>(effect => effect.Rarity);
        }
        protected override void OnClear()
        {
            
            ConcatableRarityIndex.Clear();
        }

        public override void OnAdd(EffectCommand item) {
            if (item is IConcatableEffect effect) {
                ConcatableRarityIndex.Add(effect);
            }
            
        }

        public override void OnRemove(EffectCommand item)
        {
            if (item is IConcatableEffect effect) {
                ConcatableRarityIndex.Remove(effect);
            }
        }
    }

    public interface IEffectClassificationModel : IModel {
        EffectTable AllConcatableEffects { get; }
        Dictionary<CardType, EffectTable> ConcatableEffectsByCardType { get; } 
   

        IEnumerable<IConcatableEffect> GetAllConcatableEffectsByRarity(Rarity rarity);
    }
    public class EffectClassificationModel: AbstractModel, IEffectClassificationModel {
        protected override void OnInit() {
            RegisterAllConcatableEffects();

        }

        private void RegisterAllConcatableEffects() {
            RegisterConcatableEffectToTable<DealDamageToSelf>(CardType.Character);
            RegisterConcatableEffectToTable<NormalAttackCommand>(CardType.Attack);
            RegisterConcatableEffectToTable<AddPropertyValueEffect>(CardType.Skill);
            RegisterConcatableEffectToTable<NextCharResurrectionEffect>();
            RegisterConcatableEffectToTable<DrawCardsEffect>(CardType.Skill);
            RegisterConcatableEffectToTable<OnFriendlyCharacterHurtEffect>(CardType.Area);
            RegisterConcatableEffectToTable<DealDamage2RandomEnemy>(CardType.Skill);
        }





        /// <summary>
        /// Register a concatable effect, this effect is only applicable for certain type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="onlyApplicableCardType"></param>
        private void RegisterConcatableEffectToTable<T>(CardType onlyApplicableCardType) where T : EffectCommand, IConcatableEffect, new() {
            RegisterEffectToTable<T>(AllConcatableEffects);
            RegisterEffectToTable<T>(ConcatableEffectsByCardType[onlyApplicableCardType]);
        }

        /// <summary>
        /// egister a concatable effect, this effect is applicable for all types except characters
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private void RegisterConcatableEffectToTable<T>() where T:  EffectCommand, IConcatableEffect, new(){
            //RegisterEffectToTable<T>(AllConcatableEffects);
            foreach (KeyValuePair<CardType, EffectTable> keyValuePair in ConcatableEffectsByCardType) {
                if (keyValuePair.Key != CardType.Character) {
                    RegisterConcatableEffectToTable<T>(keyValuePair.Key);
                }
            }
        }
        private void RegisterEffectToTable<T>(EffectTable effectTable) where T : EffectCommand, new() {
            effectTable.Add(EffectCommand.AllocateEffect<T>());
        }

        public EffectTable AllConcatableEffects { get; } = new EffectTable();

        public Dictionary<CardType, EffectTable> ConcatableEffectsByCardType { get; } =
            new Dictionary<CardType, EffectTable>() {
                {CardType.Character, new EffectTable()},
                {CardType.Area, new EffectTable()},
                {CardType.Armor, new EffectTable()},
                {CardType.Attack, new EffectTable()},
                {CardType.Skill, new EffectTable()},
            };

        /// <summary>
        /// Include multi rarity
        /// </summary>
        /// <param name="rarity"></param>
        /// <returns></returns>
        public IEnumerable<IConcatableEffect> GetAllConcatableEffectsByRarity(Rarity rarity) {
            return AllConcatableEffects.ConcatableRarityIndex.Get((effectRarity => effectRarity == rarity));
        }
    }
}
