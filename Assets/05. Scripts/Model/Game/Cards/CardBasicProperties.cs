using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MainGame
{
    [Serializable]
    public struct CardDisplayInfo {
        public Texture2D CardIllustration;
        public Vector3 IllustrationPos;
        public Vector2 IllustrationSize;
    }

    [Serializable]
    public class CardProperties {
        public string ID;
        public CardType CardType;
        public string NameKey;
        public int Cost;
        public Rarity rarity;
        public CardDisplayInfo CardDisplayInfo;
        public CharacterCardProperties CharacterProperties;
        public string CardScriptType;
        public Object CardScriptObject;

        public int GetID() {
            return int.Parse(ID);
        }

        public Type GetConcreteType() {
            return Type.GetType($"MainGame.{CardScriptType}");
        }
    }


    [CreateAssetMenu(fileName = "CardProperties")]
    public class CardBasicProperties : ScriptableObject {
        public List<CardProperties> CardDatas;
    }
}
