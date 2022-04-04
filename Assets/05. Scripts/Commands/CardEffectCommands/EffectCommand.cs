using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.Pool;
using Polyglot;
using UnityEngine;

namespace MainGame
{

    public enum EffectBattleEventTriggerType {
        TriggerWhenDealt,
        TriggerByOtherEventsAfterDealt,
        AlwaysTriggerByOtherEvents
    }

    [ES3Serializable]
    public abstract class EffectCommand : ICommand, ICanRegisterEvent
    {
        //Register Effect effective event
        
        private IArchitecture architectureModel;
        [ES3NonSerializable]
        public CardInfo CardBelongTo;

        public List<KeywordInfo> GetAllKeywords() {
            //Activator.CreateInstance(typeof(CardInfo))
            string localizedText = GetLocalizedTextWithoutBold();
            return this.SendQuery<List<KeywordInfo>>(new GetKeywordInfoForText(localizedText));
        }

        /// <summary>
        /// With bold keywords
        /// </summary>
        public string LocalizedEffectText {
            get {
                string localizedText = GetLocalizedTextWithoutBold();
                return this.SendQuery<string>(new GetTextWithBoldKeywordQuery(localizedText));
            }
        }

        IArchitecture IBelongToArchitecture.GetArchitecture()
        {
            return CardGame.Interface;
        }

        void ICanSetArchitecture.SetArchitecture(IArchitecture architecture)
        {
            this.architectureModel = architecture;
        }

        public void UnregisterBattleEventListeners() {
            switch (BattleEventTriggerType)
            {
                case EffectBattleEventTriggerType.TriggerWhenDealt:
                    this.GetSystem<IBattleEventControlSystem>()
                        .UnRegisterEffectFromBattleEvent(typeof(OnCardDealt), OnCardDealt);
                    break;
                case EffectBattleEventTriggerType.TriggerByOtherEventsAfterDealt:
                    this.UnRegisterEvent<IBattleEvent>(OnRegisteredBattleEventTriggered);
                    break;
                case EffectBattleEventTriggerType.AlwaysTriggerByOtherEvents:
                    this.UnRegisterEvent<IBattleEvent>(OnRegisteredBattleEventTriggered);
                    break;
            }
        }

        public void RegisterBattleEventListeners() {
            switch (BattleEventTriggerType) {
                case EffectBattleEventTriggerType.TriggerWhenDealt:
                    this.GetSystem<IBattleEventControlSystem>()
                        .RegisterEffectToBattleEvent(typeof(OnCardDealt), OnCardDealt);
                    break;
                case EffectBattleEventTriggerType.TriggerByOtherEventsAfterDealt:
                    this.GetSystem<IBattleEventControlSystem>()
                        .RegisterEffectToBattleEvent(typeof(OnCardDealt), OnCardDealt);
                    break;
                case EffectBattleEventTriggerType.AlwaysTriggerByOtherEvents:
                    this.RegisterEvent<IBattleEvent>(OnRegisteredBattleEventTriggered);
                    break;
            }
        }

        private void OnCardDealt(IBattleEvent e) {
            if (((OnCardDealt) e).CardDealt == CardBelongTo) {
                if (BattleEventTriggerType == EffectBattleEventTriggerType.TriggerWhenDealt) {
                    OnExecute();
                }

                if (BattleEventTriggerType == EffectBattleEventTriggerType.TriggerByOtherEventsAfterDealt) {
                    this.RegisterEvent<IBattleEvent>(OnRegisteredBattleEventTriggered);
                }
            }
        }

        private void OnRegisteredBattleEventTriggered(IBattleEvent e) {
            if (e.GetType() == BattleEventTypeRegister) {
                OnExecute();
            }
        }


        void ICommand.Execute() {
            OnExecute();
        }


        public virtual EffectCommand Clone() {
            EffectCommand cloned = OnCloned();
            cloned.BattleEventTriggerType = BattleEventTriggerType;
            cloned.BattleEventTypeRegister = BattleEventTypeRegister;
            return cloned;
        }

        public abstract EffectCommand OnCloned();

        [ES3Serializable]
        public abstract EffectBattleEventTriggerType BattleEventTriggerType { get; protected set; }

        /// <summary>
        /// This is only valid when the effect is trigged by other events. If this effect is not triggered by other events, use null instead
        /// </summary>
        [ES3NonSerializable]
        public abstract Type BattleEventTypeRegister { get; protected set; } 

        public EffectCommand(){}

        public abstract string GetLocalizedTextWithoutBold();

        protected string Localization(string key, params string[] formats) {
            return Polyglot.Localization.GetFormat(key, formats);
        }

        /// <summary>
        /// When this Effect is executed. The Effect is executed when its battle event triggers
        /// </summary>
        /// <param name="parameters"></param>
        protected abstract void OnExecute();

        protected abstract void Revoke();


        public virtual void OnRecycled()
        {
            architectureModel = null;
        }

        public bool IsRecycled { get; set; }

        public abstract void RecycleToCache();

    }

    [ES3Serializable]
    public abstract class BuffEffect : EffectCommand {
        [ES3Serializable]
        public BuffType BuffType;

        public override EffectCommand Clone() {
            EffectCommand cloned =  base.Clone();
          
            ((BuffEffect) cloned).BuffType = BuffType;
            return cloned;
        }

       
    }
}
