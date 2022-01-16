using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.BindableProperty;
using MikroFramework.Pool;
using MikroFramework.ResKit;
using Polyglot;
using UnityEngine;

namespace MainGame
{
    public enum CardType {
        Attack,
        Armor,
        Skill,
        Area,
        Character
    }

    public enum Rarity {
        Normal,
        Rare,
        Epic
    }




    //initial: original
    //primitive: entire game (after global buff)
    //self: only in the battle
    [ES3Serializable]
    public class CardAlterableProperty<T>{
        [ES3Serializable]
        public T PrimitiveValue;
        [ES3Serializable]
        public BindableProperty<T> CurrentValue = new BindableProperty<T>();
        public CardAlterableProperty() { }
        public CardAlterableProperty(T initialValue) {
            this.PrimitiveValue = initialValue;
            this.CurrentValue.Value = initialValue;
        }

        public void ResetToPrimitive() {
            this.CurrentValue.Value = PrimitiveValue;
        }

    }

    [ES3Serializable]
    public abstract class CardInfo : ICanSendCommand {
        [ES3Serializable]
        public int ID;

        [ES3Serializable] 
        public string NameKey;

        [ES3Serializable]
        public CardAlterableProperty<int> CostProperty;
        [ES3Serializable]
        public CardAlterableProperty<List<EffectCommand>> EffectsProperty;

        [ES3Serializable] public CardAlterableProperty<List<EffectCommand>> BuffEffectProperty;
       

        [ES3Serializable]
        public CardType CardType { get; protected set; }

        [ES3Serializable]
        public Rarity CardRarity { get; protected set; }

        public CardInfo() { }

        public CardInfo(CardProperties attributes) {
            CostProperty = new CardAlterableProperty<int>(attributes.Cost);
            CardRarity = attributes.rarity;
            CardType = attributes.CardType;
            NameKey = attributes.NameKey;
           
            SetInitialEffects();
            SetInitialPrimitiveBuffEffects();

            SetEffectsToPrimitive();
        }

       
        protected void SetEffectsToPrimitive()
        {
            if (EffectsProperty.CurrentValue.Value == EffectsProperty.PrimitiveValue)
            {
                EffectsProperty.CurrentValue.Value = new List<EffectCommand>();
            }
            else
            {
                EffectsProperty.CurrentValue.Value.ForEach(effect => effect.RecycleToCache());
                EffectsProperty.CurrentValue.Value.Clear();
            }


            EffectsProperty.PrimitiveValue.ForEach(value => {
                EffectsProperty.CurrentValue.Value.Add(value.Clone());
            });

            if (BuffEffectProperty.CurrentValue.Value == BuffEffectProperty.PrimitiveValue) {
                BuffEffectProperty.CurrentValue.Value = new List<EffectCommand>();
            }
            else {
                BuffEffectProperty.CurrentValue.Value.ForEach(effect => effect.RecycleToCache());
                BuffEffectProperty.CurrentValue.Value.Clear();
            }

            BuffEffectProperty.PrimitiveValue.ForEach(value => {
                BuffEffectProperty.CurrentValue.Value.Add(value.Clone());
            });
        }

        public abstract void SetInitialEffects();
        public abstract void SetInitialPrimitiveBuffEffects();

        public virtual string GetLocalizedDescription() {
            string description = "";
            for (int i = 0; i < BuffEffectProperty.PrimitiveValue.Count; i++) {
                description += BuffEffectProperty.PrimitiveValue[i].LocalizedEffectText;
                if (i != BuffEffectProperty.PrimitiveValue.Count - 1) {
                    description += Localization.Get("Comma");
                }
            }

            description += "\n";
            foreach (EffectCommand effectCommand in EffectsProperty.PrimitiveValue) {
                description += effectCommand.LocalizedEffectText;
            }

            return description;
        }
        /*
        public void DealCard() {
            foreach (Effect concreteEffect in ConcreteEffects) {
                this.SendCommand(concreteEffect.EffectCommand);
                concreteEffect.Callback?.Invoke();
            }
            OnCardDealt();
        }

        public abstract void OnCardDealt();
        */

        /// <summary>
        /// When the battle is ended
        /// </summary>
        public void ResetToPrimitive() {
            CostProperty.ResetToPrimitive();
            SetEffectsToPrimitive();
            OnResetToPrimitive();
        }

     

        public virtual void OnResetToPrimitive() { }
       

        public IArchitecture GetArchitecture() {
            return CardGame.Interface;
        }

       
    }
}
