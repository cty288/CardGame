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
using UnityEngine.UI;

namespace MainGame
{
    public class CardDisplay : PoolableGameObject, ICanGetModel {
        public CardType CardType;
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

        private void Awake() {
            CardInfo = new BindableProperty<CardInfo>();
            CardNameText = transform.Find("CardName").GetComponent<TMP_Text>();
            CardIllustraion = transform.Find("CardIllustrationMask/CardIllustraion").GetComponent<RawImage>();
            CardRarityTagImage = transform.Find("CardRarityTagImage").GetComponent<Image>();
            CardDescriptionText = transform.Find("CardDescription").GetComponent<TMP_Text>();
           
            CardInfo.RegisterOnValueChaned(OnCardInfoChange).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void OnCardInfoChange(CardInfo prevCardInfo, CardInfo curCardInfo) {
            RefreshCardDisplay();
        }


        private void RefreshCardDisplay() {
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
            
            switch (CardType) {
                case CardType.Character:
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
                    CostText.text = CardInfo.Value.CostProperty.CurrentValue.Value.ToString();
                    CardTypeText.text = CardType.ToString();
                    break;
            }
        }
        public override void OnInit() {
            
        }

        public override void OnRecycled() {
            transform.SetParent(Pool.transform);
        }

        public IArchitecture GetArchitecture() {
            return CardGame.Interface;
        }
    }
}
