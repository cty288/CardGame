using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MikroFramework.CodeGen;
using Polyglot;
using TMPro;
#if  UNITY_EDITOR
using MikroFramework;
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
        public string CodeGenerationRootPath;
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




            //script.CardScriptType = script.scriptType.name;
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Code Generation Root Path", new GUIStyle()
            {
                alignment = TextAnchor.LowerLeft,
                fontSize = 12,
            });

            script.CodeGenerationRootPath = EditorGUILayout.TextField(
                EditorPrefs.GetString("CodeGenRootPath",
                    EditorPrefs.GetString("CodeGenerationRootPath", Application.dataPath + "/05. Scripts/Model/Game/Cards")));

            if (GUILayout.Button("Select Path"))
            {

                string searchPath = EditorUtility.OpenFolderPanel("select path", script.CodeGenerationRootPath, "") + "/";
                if (searchPath != "")
                {
                    EditorPrefs.SetString("CodeGenerationRootPath", searchPath);
                }
            }
            if (GUILayout.Button("Save"))
            {

                EditorPrefs.SetString("CodeGenerationRootPath", script.CodeGenerationRootPath);
            }
            EditorGUILayout.EndHorizontal();



            EditorGUILayout.BeginHorizontal();
            script.CardScriptType = EditorGUILayout.TextField("Script Class Name", script.CardScriptType);

            if (GUILayout.Button("Generate Code")) {
                Object obj = GenerateCardCode(script.CardType, script);
                Debug.Log(obj.name);
                script.scriptType = obj;

            }

            
            EditorGUILayout.EndHorizontal();
            script.scriptType =
                ((Object)EditorGUILayout.ObjectField("Script Class", script.scriptType, typeof(Object)));




            script.CardType = (CardType) EditorGUILayout.EnumPopup("CardType", script.CardType);
           
          //  Debug.Log(script.CardScriptType);
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

        private Object GenerateCardCode(CardType cardType, CardConfigure script) {
            string filePath = script.CodeGenerationRootPath + script.CardScriptType + ".cs";
            if (!File.Exists(filePath))
            {
                if (cardType == CardType.Character) {
                    File.WriteAllText(filePath, WriteCharacterCardInfo(script.CardScriptType, script));
                }
                else {
                    File.WriteAllText(filePath, WriterCardInfo(script.CardScriptType, script));
                }

                
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            int idx = script.CodeGenerationRootPath.LastIndexOf("Assets/");
            string relativePath = script.CodeGenerationRootPath.Substring(idx);
            Debug.Log(relativePath);
            return AssetDatabase.LoadAssetAtPath(relativePath+script.CardScriptType+".cs", typeof(MonoScript));
        }

        private string WriterCardInfo(string className, CardConfigure script) {
            StringBuilder code = new StringBuilder();

            WriteDefaultNamespaceCode(code);

            code.Append("\n");
            code.Append($"namespace MainGame {{\n");


            code.Append($"\tpublic class {className} : CardInfo {{\n");
            code.Append($"\tpublic {className} () {{}}\n" +
                        $"\tpublic {className}(CardProperties attributes) : base(attributes) {{}}\n" +
                        $"\tpublic override List<MikroAction> InitialEffectOverrideAnimation {{ get; protected set; }}\n" +
                        $"\tpublic override List<EffectCommand> SetInitialEffects() {{\n" +
                        $"\t\treturn new List<EffectCommand>() {{\n\n"+ 
                        $"\t\t}};\n"+
                        $"\t}}\n\n" +
                        $"\tpublic override List<EffectCommand> SetInitialPrimitiveBuffEffects() {{\n" +
                        $"\t\treturn new List<EffectCommand>() {{\n\n" +
                        $"\t\t}};\n" +
                        $"\t}}\n\n" +
                        $"\t public override void OnCardDealtSuccess() {{\n" +
                        $"\t\tthrow new NotImplementedException();\n" +
                        $"\t}}");
            code.Append("\n");
            code.Append("\t}\n");
            code.Append("}");
            return code.ToString();
        }

        private static string WriteCharacterCardInfo(string className, CardConfigure script)
        {
            StringBuilder code = new StringBuilder();
            
            WriteDefaultNamespaceCode(code);

            code.Append("\n");
            code.Append($"namespace MainGame {{\n");


            code.Append($"\tpublic class {className} : CharacterCardInfo {{\n");
            code.Append($"\tpublic {className} () {{}}\n" +
                        $"\tpublic {className}(CardProperties attributes) : base(attributes) {{}}\n" +
                        $"\tpublic override List<MikroAction> InitialEffectOverrideAnimation {{ get; protected set; }}\n" +
                        $"\tpublic override List<EffectCommand> SetInitialEffects() {{\n" +
                        $"\t\treturn new List<EffectCommand>() {{\n\n" +
                        $"\t\t}}\n;" +
                        $"\t}}\n\n" +
                        $"\tpublic override List<EffectCommand> SetInitialPrimitiveBuffEffects() {{\n" +
                        $"\t\treturn new List<EffectCommand>() {{\n\n" +
                        $"\t\t}}\n;" +
                        $"\t}}");
            code.Append("\n");
            code.Append("\t}\n");
            code.Append("}");
            return code.ToString();
        }

        private static void WriteDefaultNamespaceCode(StringBuilder code)
        {
            BindingDefaultNamespaces.DefaultScriptNamespaces.ForEach(namespaceName => {
                code.Append($"using {namespaceName};\n");
            });
        }
    }
#endif
}
