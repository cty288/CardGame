using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework;
using MikroFramework.ActionKit;
using MikroFramework.Architecture;
using MikroFramework.Pool;
using Polyglot;
using UnityEngine;
using Random = System.Random;

namespace MainGame
{

  
    public interface ITriggeredWhenDealt {

    }

    public interface IAlwaysTriggeredByOtherEvents {
        public List<IBattleEvent> TriggeredBy { get; set; }
        public List<EffectCommand> TriggeredEffects { get; set; }
    }

    public interface ITriggeredByEventsWhenDealt {
        public List<IBattleEvent> TriggeredBy { get; set; }
        public List<EffectCommand> TriggeredEffects { get; set; }
    }
    
    public interface IConcatableEffect : ICommand {
        
        public Rarity Rarity { get; set; }
        /// <summary>
        /// Can be negative
        /// </summary>
        public int CostValue { get; }

        /// <summary>
        /// Before actually randomly generate this effect. This is usually used to setup some random factors for calculate the cost
        /// </summary>
        public void OnGenerationPrep();


    }

    [ES3Serializable]
    public abstract class EffectCommand :  ICommand, ICanRegisterEvent {
        [ES3Serializable] public int SerializeCode = UnityEngine.Random.Range(-10000000, 1000000);
        [ES3Serializable] public abstract bool IsBuffEffect { get; set; }
        public static T AllocateEffect<T>() where T : EffectCommand, new() {
            return SafeObjectPool<T>.Singleton.Allocate();
        }
        public static void RecycleEffect<T>(T cmd) where T : EffectCommand, new() {
             SafeObjectPool<T>.Singleton.Recycle(cmd);
        }
        //Register Effect effective event

        private IArchitecture architectureModel;
        [ES3NonSerializable]
        public CardInfo CardBelongTo;


        [ES3NonSerializable]
        protected MikroAction executeAnimationEffect;


        public CardDisplay GetCardDisplayBelongTo() {
            return CardBelongTo.CardDisplayBelongTo;
        }
        public void OverrideExecuteAnimationEffect(MikroAction effect) {
            this.executeAnimationEffect = effect;
        }
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

            if (this is ITriggeredWhenDealt)
            {
                this.GetSystem<IBattleEventControlSystem>()
                    .UnRegisterEffectFromBattleEvent(typeof(OnCardDealt), OnCardDealt);
            }

            if (this is ITriggeredByEventsWhenDealt)
            {
                this.UnRegisterEvent<IBattleEvent>(OnRegisteredBattleEventTriggered);
            }

            if (this is IAlwaysTriggeredByOtherEvents)
            {
                this.UnRegisterEvent<IBattleEvent>(OnRegisteredBattleEventTriggered);
            }
        }

        public void RegisterBattleEventListeners() {
            if (this is ITriggeredWhenDealt) {
                this.GetSystem<IBattleEventControlSystem>()
                    .RegisterEffectToBattleEvent(typeof(OnCardDealt), OnCardDealt);
            }

            if (this is ITriggeredByEventsWhenDealt dealt) {
                this.GetSystem<IBattleEventControlSystem>()
                    .RegisterEffectToBattleEvent(typeof(OnCardDealt), OnCardDealt);
            }

            if (this is IAlwaysTriggeredByOtherEvents) {
                this.RegisterEvent<IBattleEvent>(OnRegisteredBattleEventTriggered);
            }
        }

        private void OnCardDealt(IBattleEvent e) {
            Debug.Log("On Card Dealt");
            if (((OnCardDealt) e).CardDealt == CardBelongTo) {
                if (this is ITriggeredWhenDealt) {
                    Execute();
                }

                if (this is ITriggeredByEventsWhenDealt listener) 
                {
                    this.RegisterEvent<IBattleEvent>(OnRegisteredBattleEventTriggered);
                }
            }
        }

        private void OnRegisteredBattleEventTriggered(IBattleEvent e) {
            if (this is ITriggeredByEventsWhenDealt listener) {
                foreach (IBattleEvent battleEvent in listener.TriggeredBy) {
                    if (battleEvent.GetType() == e.GetType()) {
                        Execute();
                    }
                }
            }
            if (this is IAlwaysTriggeredByOtherEvents listener2) {
                foreach (IBattleEvent battleEvent in listener2.TriggeredBy) {
                    if (battleEvent.GetType() == e.GetType()) {
                        Execute();
                    }
                }
            }
        }


        void ICommand.Execute() {
            OnExecute();
        }

        public void Execute() {
            this.GetSystem<IBattleEventControlSystem>().RegisterAnimationToSequence(executeAnimationEffect);
            OnExecute();
        }


        public virtual EffectCommand Clone() {
            EffectCommand cloned = OnCloned();
            cloned.executeAnimationEffect = this.executeAnimationEffect;
            return cloned;
        }

        public abstract EffectCommand OnCloned();





        public EffectCommand() {
            if (executeAnimationEffect == null) {
                executeAnimationEffect = this.GetExecuteAnimationEffect();
            }
           
        }

        public abstract MikroAction GetExecuteAnimationEffect();

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

    

}
