using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.ActionKit;
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
        Epic,
        Legend,
        Multi
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
        public CardAlterableProperty(T initialValue, Func<T,T> cloneMethod) {
            this.PrimitiveValue = initialValue;
            this.CurrentValue.Value = cloneMethod(initialValue);
        }

        public void ResetToPrimitive() {
            this.CurrentValue.Value = PrimitiveValue;
        }

    }

    [ES3Serializable]
    public abstract class CardInfo : ICanSendCommand, ICanRegisterEvent {
        [ES3Serializable]
        public int ID;

        [ES3Serializable] 
        public string NameKey;

        [ES3Serializable]
        public CardAlterableProperty<int> CostProperty;
        [ES3Serializable]
        public CardAlterableProperty<List<EffectCommand>> EffectsProperty;

        [ES3Serializable] public CardAlterableProperty<List<EffectCommand>> BuffEffectProperty;
        
        [ES3NonSerializable]
        public abstract List<MikroAction> InitialEffectOverrideAnimation { get; protected set; }


        [ES3Serializable]
        public CardType CardType { get; protected set; }

        [ES3Serializable]
        public Rarity CardRarity { get; protected set; }

        [ES3NonSerializable] public CardDisplay CardDisplayBelongTo { get; set; }

        public CardInfo() {
            this.RegisterEvent<IBattleEvent>(OnEnterBattleScene);
            this.RegisterEvent<IBattleEvent>(OnLeaveBattleScene);
            //InitialEffectAnimationOverride();
            Debug.Log("OnEnterBattleScene Event Registered");
        }

        public CardInfo(CardProperties attributes) {
            CostProperty = new CardAlterableProperty<int>(attributes.Cost, i => i );
            CardRarity = attributes.rarity;
            CardType = attributes.CardType;
            NameKey = attributes.NameKey;
            this.RegisterEvent<IBattleEvent>(OnEnterBattleScene);
            this.RegisterEvent<IBattleEvent>(OnLeaveBattleScene);
            // this.RegisterEvent<OnEnterBattleScene>(OnEnterBattleScene);
            // this.RegisterEvent<OnLeaveBattleScene>(OnLeaveBattleScene);

            EffectsProperty = new CardAlterableProperty<List<EffectCommand>>(SetInitialEffects(), list => {
                List<EffectCommand> newEffect = new List<EffectCommand>();
                foreach (EffectCommand effectCommand in list) {
                    newEffect.Add(effectCommand.Clone());
                    //effectCommand.RecycleToCache();
                }

                return newEffect;
            });

            BuffEffectProperty = new CardAlterableProperty<List<EffectCommand>>(SetInitialPrimitiveBuffEffects(),
                list => {
                    List<EffectCommand> newEffect = new List<EffectCommand>();
                    foreach (EffectCommand effectCommand in list) {
                        newEffect.Add(effectCommand.Clone());
                       // effectCommand.RecycleToCache();
                    }

                    return newEffect;
                });

            SetEffectsToPrimitive();
        }

        private void InitialEffectAnimationOverride() {
            if (InitialEffectOverrideAnimation != null && InitialEffectOverrideAnimation.Count > 0) {
                for (int i = 0; i < EffectsProperty.PrimitiveValue.Count; i++) {
                    if (i < InitialEffectOverrideAnimation.Count) {
                        // EffectsProperty.p
                        EffectsProperty.PrimitiveValue[i]
                            .OverrideExecuteAnimationEffect(InitialEffectOverrideAnimation[i]);
                    }
                }
            }
           
        }

        protected void OnLeaveBattleScene(IBattleEvent e) {
            if (e is OnLeaveBattleScene) {
                ResetToPrimitive();
            }
          
        }

        /// <summary>
        /// Let effects register battle events
        /// </summary>
        /// <param name="obj"></param>
        protected void OnEnterBattleScene(IBattleEvent e) {
            if (e is OnEnterBattleScene) {
                ResetToPrimitive();
                Debug.Log("Enter Battle Scene");
                EffectsProperty.CurrentValue.Value.ForEach(cmd => {
                    if (cmd.CardBelongTo == null) cmd.CardBelongTo = this;
                    cmd.RegisterBattleEventListeners();
                });

                BuffEffectProperty.CurrentValue.Value.ForEach(cmd => {
                    if (cmd.CardBelongTo == null) cmd.CardBelongTo = this;
                    cmd.RegisterBattleEventListeners();
                });
            }
          
        }


        protected void SetEffectsToPrimitive()
        {
            InitialEffectAnimationOverride();
            EffectsProperty.CurrentValue.Value.ForEach(cmd => {
                cmd.UnregisterBattleEventListeners();
            });
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
                EffectCommand cmd = value.Clone();
                cmd.CardBelongTo = this;
                EffectsProperty.CurrentValue.Value.Add(cmd);

            });




             
            BuffEffectProperty.CurrentValue.Value.ForEach(cmd => {
                cmd.UnregisterBattleEventListeners();
            });
            if (BuffEffectProperty.CurrentValue.Value == BuffEffectProperty.PrimitiveValue) {
                BuffEffectProperty.CurrentValue.Value = new List<EffectCommand>();
            }
            else {
                BuffEffectProperty.CurrentValue.Value.ForEach(effect => effect.RecycleToCache());
                BuffEffectProperty.CurrentValue.Value.Clear();
            }

            BuffEffectProperty.PrimitiveValue.ForEach(value => {
                EffectCommand cmd = value.Clone();
                cmd.CardBelongTo = this;
                BuffEffectProperty.CurrentValue.Value.Add(cmd);
            });
        }

        public abstract List<EffectCommand> SetInitialEffects();
        public abstract List<EffectCommand> SetInitialPrimitiveBuffEffects();

        public virtual string GetLocalizedDescription() {
            string description = "";
            //buff effect: keywords 
        
            for (int i = 0; i < BuffEffectProperty.PrimitiveValue.Count; i++) {
                Debug.Log(NameKey);
                Debug.Log(BuffEffectProperty.PrimitiveValue[0] == null);
                description += BuffEffectProperty.PrimitiveValue[i].LocalizedEffectText;
                if (i != BuffEffectProperty.PrimitiveValue.Count - 1) {
                    description += Localization.Get("Comma");
                }
            }

            description += "\n";
            foreach (EffectCommand effectCommand in EffectsProperty.PrimitiveValue) {
                description += effectCommand.LocalizedEffectText + Localization.Get("Period");
            }

            return description;
        }
        
        public void TryDealCard() {
            Debug.Log("Try Deal Card");
            this.SendCommand<TryDealCardCommand>(new TryDealCardCommand(this));
          
        }

        public abstract void OnCardDealtSuccess();

       
        

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
