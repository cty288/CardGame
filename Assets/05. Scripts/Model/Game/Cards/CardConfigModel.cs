using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MikroFramework.Architecture;
using MikroFramework.DataStructures;
using MikroFramework.ResKit;
using UnityEngine;

namespace MainGame
{
    public interface ICardConfigModel : IModel {
        CardInfo GetNewCardInfoFromID(int id);
        CardDisplayInfo GetCardDisplayInfo(int id);
    }

   
    public class CardTable<T> : Table<CardProperties> where T: CardInfo { 
        public TableIndex<int, CardProperties> IDIndex { get; private set; }

        public CardTable() {
            IDIndex = new TableIndex<int, CardProperties>(item => item.GetID());
        }
        protected override void OnClear() {
            IDIndex.Clear();
        }

        public override void OnAdd(CardProperties item) {
            IDIndex.Add(item);
        }

        public override void OnRemove(CardProperties item) {
           IDIndex.Remove(item);
        }

        public CardProperties GetByID(int id) {
           return IDIndex.Get(id).FirstOrDefault();
        }
    }

    public class CardConfigModel : AbstractModel, ICardConfigModel {

        public CardTable<CardInfo> AllCardInfos = new CardTable<CardInfo>();
        public CardTable<CharacterCardInfo> AllCharacterCardInfos = new CardTable<CharacterCardInfo>();
        private ResLoader resLoader;

        private List<CardProperties> cardPropertiesConfig;
        //Character: 0-9999
        //Attack: 10000 - 19999
        //Armor: 20000 - 29999
        //Skill: 30000 - 39999
        //Area: 40000 - 49999

        protected override void OnInit() {
            resLoader = new ResLoader();
            RegisterCardInfo();
        }

        
        public void RegisterCardInfo() {
            //RegisterCharacterCardInfo(0, "TestCharacter",0, 100, 10,50, Rarity.Normal, CharacterType.Warrior, typeof(TestCharacter));
            CardBasicProperties cardBasicPropertiesConfig = resLoader.LoadSync<CardBasicProperties>("card_properties", "CardProperties");
            cardPropertiesConfig = cardBasicPropertiesConfig.CardDatas;

            foreach (CardProperties cardProperties in cardPropertiesConfig) {
                if (cardProperties.CardType == CardType.Character) {
                    RegisterCharacterCardInfo(cardProperties);
                }
            }
        }

        public CardDisplayInfo GetCardDisplayInfo(int id) {
            return AllCharacterCardInfos.GetByID(id).CardDisplayInfo;
        }

        public CardInfo GetNewCardInfoFromID(int id) {
            CardProperties cardAttributes = AllCardInfos.GetByID(id);
            return Activator.CreateInstance(cardAttributes.GetConcreteType(), cardAttributes) as CardInfo;
        }

        private void RegisterCharacterCardInfo(CardProperties cardProperties) {
            AllCardInfos.Add(cardProperties);
            AllCharacterCardInfos.Add(cardProperties);
        }

    }
}
