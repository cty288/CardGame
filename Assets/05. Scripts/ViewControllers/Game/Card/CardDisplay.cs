using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.BindableProperty;
using MikroFramework.Event;
using MikroFramework.Pool;
using Polyglot;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MainGame
{
    public class CardDisplay : PoolableGameObject, ICanGetModel, IPointerClickHandler {
       // public CardType CardType;
        [HideInInspector]
        public BindableProperty<CardInfo> CardInfo;

        [HideInInspector] public CardDisplayInfo CardDisplayInfo;

        [HideInInspector]
        public int CardID;

        public TMP_Text CostText;
        [HideInInspector]
        public TMP_Text CardNameText;
        [HideInInspector]
        public RawImage CardIllustraion;
        [HideInInspector]
        public Image CardRarityTagImage;
        public Sprite[] CardRarityTagSprites;
        [HideInInspector]
        public TMP_Text CardDescriptionText;
        public TMP_Text CardAttackText;
        public TMP_Text CardHealthText;
        public Image CharacterCardTypeImage;
        public Sprite[] CharacterCardTypeSprites;
        public Slider CardStrengthSlider;
        public TMP_Text CardTypeText;
        public Image CardBG;
        public Sprite CharacterCardSprite;
        public Sprite NormalCardSprite;

        public Action<CardDisplay> OnClicked;
        private void Awake() {
            CardInfo = new BindableProperty<CardInfo>();
            CardNameText = transform.Find("CardName").GetComponent<TMP_Text>();
            CardIllustraion = transform.Find("CardIllustrationMask/CardIllustraion").GetComponent<RawImage>();
            CardRarityTagImage = transform.Find("CardRarityTagImage").GetComponent<Image>();
            CardDescriptionText = transform.Find("CardDescription").GetComponent<TMP_Text>();
            CostText = transform.Find("Cost").GetComponent<TMP_Text>();
            CardTypeText = transform.Find("CardTypeName").GetComponent<TMP_Text>();
            CardBG = transform.Find("CardBG").GetComponent<Image>();
            CardInfo.RegisterOnValueChaned(OnCardInfoChange).UnRegisterWhenGameObjectDestroyed(gameObject);
          
        }

     
        private void OnCardInfoChange(CardInfo prevCardInfo, CardInfo curCardInfo) {
            RefreshCardDisplay();
        }


        public void RefreshCardDisplay() {
            CardID = CardInfo.Value.ID;
            CardDescriptionText.text = CardInfo.Value.GetLocalizedDescription();
            CardNameText.text = Localization.Get(CardInfo.Value.NameKey);
            //Card Illustraion
            CardDisplayInfo = this.GetModel<ICardConfigModel>().GetCardDisplayInfo(CardID);
            CardIllustraion.texture = CardDisplayInfo.CardIllustration;
            CardIllustraion.GetComponent<RectTransform>().localPosition = CardDisplayInfo.IllustrationPos;
            CardIllustraion.GetComponent<RectTransform>().sizeDelta = CardDisplayInfo.IllustrationSize;

            if (CardRarityTagSprites.Length > 0) {
                CardRarityTagImage.sprite = CardRarityTagSprites[(int)CardInfo.Value.CardRarity];
            }
            
            switch (CardInfo.Value.CardType) {
                case CardType.Character:
                    CardAttackText.gameObject.SetActive(true);
                    CardHealthText.gameObject.SetActive(true);
                    CardStrengthSlider.gameObject.SetActive(true);
                    CharacterCardTypeImage.gameObject.SetActive(true);

                    CardTypeText.gameObject.SetActive(false);
                    CostText.gameObject.SetActive(false);
                    CardBG.sprite = CharacterCardSprite;
                    CharacterCardInfo characterCardInfo = CardInfo.Value as CharacterCardInfo;
                    CardAttackText.text = characterCardInfo.Attack.CurrentValue.ToString();
                    CardHealthText.text = characterCardInfo.Health.CurrentValue.ToString();
                    CardStrengthSlider.value = (float) characterCardInfo.Strength.CurrentValue.Value /
                                               characterCardInfo.Strength.PrimitiveValue;
                    CardStrengthSlider.transform.Find("StrengthIndication").GetComponent<TMP_Text>().text =
                        $"{characterCardInfo.Strength.CurrentValue.Value}/{characterCardInfo.Strength.PrimitiveValue}";

                    if (CharacterCardTypeSprites.Length > 0) {
                        CharacterCardTypeImage.sprite = CharacterCardTypeSprites[(int) characterCardInfo.CharacterType];
                    }

                    break;
                default:
                    CardTypeText.gameObject.SetActive(true);
                    CostText.gameObject.SetActive(true);
                    CardBG.sprite = NormalCardSprite;
                    CardAttackText.gameObject.SetActive(false);
                    CardHealthText.gameObject.SetActive(false);
                    CardStrengthSlider.gameObject.SetActive(false);
                    CharacterCardTypeImage.gameObject.SetActive(false);
                    Debug.Log(CardInfo.Value.NameKey);
                    Debug.Log(CardInfo.Value.CostProperty.CurrentValue);
                    CostText.text = CardInfo.Value.CostProperty.CurrentValue.Value.ToString();
                    CardTypeText.text = GetCardTypeLocalized(CardInfo.Value.CardType);
                    break;
            }
        }

        public static string GetCardTypeLocalized(CardType type) {
            switch (type) {
                case CardType.Character:
                    return Localization.Get("cardtype_character");
                    break;
                case CardType.Skill:
                    return Localization.Get("cardtype_skill");
                    break;
                case CardType.Armor:
                    return Localization.Get("cardtype_armor");
                    break;
                case CardType.Attack:
                    return Localization.Get("cardtype_attack");
                    break;
                case CardType.Area:
                    return Localization.Get("cardtype_area");
                    break;
            }

            return "";
        }
        public override void OnInit() {
            
        }

        public override void OnRecycled() {
            transform.SetParent(Pool.transform);
        }

        public IArchitecture GetArchitecture() {
            return CardGame.Interface;
        }

        public void OnPointerClick(PointerEventData eventData) {
            OnClicked?.Invoke(this);
            Debug.Log("Pointer clicked");
        }
    }
}
