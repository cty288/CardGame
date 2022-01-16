using System.Collections;
using System.Collections.Generic;
using MikroFramework;
using MikroFramework.Architecture;
using Polyglot;
using UnityEngine;

namespace MainGame
{
    public class GetTextWithBoldKeywordQuery : AbstractQuery<string> {
        public string localizedText;

        public GetTextWithBoldKeywordQuery(string localizedText) {
            this.localizedText = localizedText;
        }

        protected override string OnDo() {
          
            List<KeywordInfo> keywordInfos =  this.GetModel<IKeywordConfigModel>().KeyWordInfos;
            foreach (KeywordInfo keywordInfo in keywordInfos) {
                string localizedKeyword = Localization.Get(keywordInfo.LocalizationKey);

                if (localizedText.Contains(localizedKeyword)) {
                    localizedText = localizedText.Replace(localizedKeyword, $"<b>{localizedKeyword}</b>");
                }
            }

            return localizedText;
        }
    }
}
