using System;
using System.Collections;
using System.Collections.Generic;
using Polyglot;
using TMPro;
#if  UNITY_EDITOR
using MikroFramework.ResKit;
using UnityEditor;

#endif
using UnityEngine.UI;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MainGame
{
    
    [ExecuteInEditMode]
    public class CardConfigure : MonoBehaviour {
        public int CardID;
        public string NameKey;
        public int Cost;
        public Rarity rarity;
        public string CardScriptType;

        public CardType CardType;
       
        public CharacterCardProperties CharacterProperties;

        public Texture2D CardIllustration;
        public Vector3 IllustrationPos;
        public Vector2 IllustrationSize;

        public Object scriptType;
        public GameObject characterIdentityPrefab;

        public CardBasicProperties CardProperties;

        private void Update() {
            transform.Find("CardName").GetComponent<TMP_Text>().text = Localization.Get(NameKey);
            if (transform.Find("CardCost")) {
                transform.Find("CardCost").GetComponent<TMP_Text>().text = Cost.ToString();
            }

            if (CardType == CardType.Character) {
                transform.Find("Attack").GetComponent<TMP_Text>().text = CharacterProperties.Attack.ToString();
                transform.Find("Health").GetComponent<TMP_Text>().text = CharacterProperties.Health.ToString();
                transform.Find("StrengthBar/StrengthIndication").GetComponent<TMP_Text>().text =
                    $"{CharacterProperties.Strength}/{CharacterProperties.Strength}";
            }
        }

        public void UpdateCardConfig() {
            CardProperties card = CardProperties.CardDatas.Find(card => int.Parse(card.ID) == CardID);
            if (card != null)
            {
                card.NameKey = NameKey;
                card.Cost = Cost;
                card.rarity = rarity;
                card.CardScriptType = CardScriptType;
                card.CardType = CardType;
                card.CharacterProperties = CharacterProperties;
                card.CardScriptObject = scriptType;
                card.CardDisplayInfo = new CardDisplayInfo() {
                    CardIllustration = CardIllustration,
                    IllustrationPos = IllustrationPos,
                    IllustrationSize = IllustrationSize
                };
                card.CharacterCardPrefab = characterIdentityPrefab;
            }
            else
            {
                CardProperties.CardDatas.Add(new CardProperties() {
                    ID = CardID.ToString(),
                    NameKey = NameKey,
                    Cost = Cost,
                    rarity = rarity,
                    CardScriptType = CardScriptType,
                    CardType = CardType,
                    CardScriptObject = scriptType,
                    CharacterProperties = CharacterProperties,
                    CardDisplayInfo = new CardDisplayInfo() {
                        CardIllustration = CardIllustration,
                        IllustrationPos = IllustrationPos,
                        IllustrationSize = IllustrationSize
                    }
                });
            }
        }
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(CardConfigure))][ExecuteInEditMode]
    public class CardIllustrationDisplaySettingsEditor : Editor {
        
        SerializedProperty characterProperties;
       
       

        private void OnEnable() {
            characterProperties = serializedObject.FindProperty("CharacterProperties");
           
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            CardConfigure script = target as CardConfigure;
            script.CardProperties = (CardBasicProperties) EditorGUILayout.ObjectField("Card Propertity Config Scripable Object",
                script.CardProperties, typeof(CardBasicProperties));
            script.CardID = EditorGUILayout.IntField("Card ID", script.CardID);
            script.NameKey = EditorGUILayout.TextField("Card Name Key", script.NameKey);
            script.Cost = EditorGUILayout.IntField("Card Cost", script.Cost);
            script.rarity =(Rarity) EditorGUILayout.EnumPopup("Card Rarity", script.rarity);
            script.scriptType =
                ((Object)EditorGUILayout.ObjectField("Script Class", script.scriptType, typeof(Object)));
            script.CardScriptType = script.scriptType.name;
           

            script.CardType = (CardType) EditorGUILayout.EnumPopup("CardType", script.CardType);
           
            Debug.Log(script.CardScriptType);
            switch (script.CardType) {
                case CardType.Character:
                    EditorGUILayout.PropertyField(characterProperties, new GUIContent("Character Properties"));
                    script.characterIdentityPrefab = (GameObject) EditorGUILayout.ObjectField(
                        "Character Card Identity Prefab", script.characterIdentityPrefab,
                        typeof(GameObject));
                    break;
                default:
                    break;

            }

            script.CardIllustration = (Texture2D)EditorGUILayout.ObjectField("Card Illustration Sprite",
                script.CardIllustration, typeof(Texture2D));


            if (GUILayout.Button("Read Sprite",
                new GUIStyle("button")
                    { fontSize = 15, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter }))
            {

                script.transform.Find("CardIllustrationMask/CardIllustraion").GetComponent<RawImage>().texture
                    = script.CardIllustration;

                AssetDatabase.Refresh();
                EditorUtility.SetDirty(script.gameObject);
            }


            if (GUILayout.Button("Save Card Properties",
                new GUIStyle("button")
                    { fontSize = 15, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter })) {
                RectTransform rect = script.gameObject.GetComponentInChildren<RawImage>().GetComponent<RectTransform>();
                script.IllustrationPos = rect.localPosition;
                script.IllustrationSize = rect.sizeDelta;
                script.UpdateCardConfig();
                Debug.Log("Save Success!");
               
            }

            if (GUILayout.Button("Read Existing Card By ID",
                new GUIStyle("button")
                    { fontSize = 15, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter })) {
                CardProperties loadedCardProperties = script.CardProperties.CardDatas.Find(card => int.Parse(card.ID) == script.CardID);
                script.NameKey = loadedCardProperties.NameKey;
                script.Cost = loadedCardProperties.Cost;
                script.rarity = loadedCardProperties.rarity;
                script.CardScriptType = loadedCardProperties.CardScriptType;
                script.scriptType = loadedCardProperties.CardScriptObject;
                script.CardType = loadedCardProperties.CardType;
                script.CharacterProperties = loadedCardProperties.CharacterProperties;
                script.CardIllustration = loadedCardProperties.CardDisplayInfo.CardIllustration;
                script.transform.Find("CardIllustrationMask/CardIllustraion").GetComponent<RawImage>().texture
                    = script.CardIllustration;
                script.characterIdentityPrefab = loadedCardProperties.CharacterCardPrefab;
                RectTransform rect = script.gameObject.GetComponentInChildren<RawImage>().GetComponent<RectTransform>();
                rect.localPosition = loadedCardProperties.CardDisplayInfo.IllustrationPos;
                rect.sizeDelta = loadedCardProperties.CardDisplayInfo.IllustrationSize;
                
            }

            serializedObject.ApplyModifiedProperties();

        }
    }
#endif
}
