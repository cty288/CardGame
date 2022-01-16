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
    [ES3Serializable]
    public abstract class EffectCommand : ICommand
    {
        //Register Effect effective event
        
        private IArchitecture architectureModel;

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
            return architectureModel;
        }

        void ICanSetArchitecture.SetArchitecture(IArchitecture architecture)
        {
            this.architectureModel = architecture;
        }


        void ICommand.Execute() {
            OnExecute();
        }


        public abstract EffectCommand Clone();

       

        public abstract string GetLocalizedTextWithoutBold();

        protected string Localization(string key, params string[] formats) {
            return Polyglot.Localization.GetFormat(key, formats);
        }

        /// <summary>
        /// Execute this command
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
            BuffEffect cloned = OnCloned();
            cloned.BuffType = BuffType;
            return cloned;
        }

        public abstract BuffEffect OnCloned();
    }
}
