using System.Collections;
using System.Collections.Generic;
using MikroFramework;
using MikroFramework.Architecture;
using Polyglot;
using UnityEngine;

namespace MainGame
{
    public class GetKeywordInfoForText : AbstractQuery<List<KeywordInfo>>
    {
        public string localizedText;

        public GetKeywordInfoForText(string localizedText)
        {
            this.localizedText = localizedText;
        }

        protected override List<KeywordInfo> OnDo()
        {
            List<KeywordInfo> results = new List<KeywordInfo>();
            List<KeywordInfo> keywordInfos = this.GetModel<IKeywordConfigModel>().KeyWordInfos;
            foreach (KeywordInfo keywordInfo in keywordInfos)
            {
                string localizedKeyword = Localization.Get(keywordInfo.LocalizationKey);

                if (localizedText.Contains(localizedKeyword) || localizedText.Contains(localizedKeyword.ToLower()))
                {
                    localizedText = localizedText.Replace(localizedKeyword, $"<b>{localizedKeyword}</b>");
                    results.Add(new KeywordInfo()
                        { DescriptionKey = keywordInfo.DescriptionKey, LocalizationKey = keywordInfo.LocalizationKey });
                }
            }

            return results;
        }
    }
}