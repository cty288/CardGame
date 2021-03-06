using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

namespace MainGame
{
    public struct KeywordInfo {
        public string LocalizationKey;
        public string DescriptionKey;
    }
    public interface IKeywordConfigModel : IModel {
        public List<KeywordInfo> KeyWordInfos { get; }
    }
    public class KeywordConfigModel : AbstractModel, IKeywordConfigModel
    {
        protected override void OnInit() {
           
        }

        public List<KeywordInfo> KeyWordInfos { get; } = new List<KeywordInfo>() {
            new KeywordInfo(){LocalizationKey = "keyword_roar", DescriptionKey = "TestKeyword1"},
            new KeywordInfo(){LocalizationKey = "keyword_attack", DescriptionKey = ""},
            new KeywordInfo(){LocalizationKey = "keyword_revive", DescriptionKey = ""}
        };
    }
}
