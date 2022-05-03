using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MikroFramework.Architecture;
using MikroFramework.DataStructures;
using MikroFramework.ResKit;
using UnityEditor;
using UnityEngine;

namespace MainGame
{
    public interface ICardConfigModel : IModel {
        CardInfo GetNewCardInfoFromID(int id);

       IEnumerable<CardProperties> GetCardProperties(Func<CardProperties, bool> contition);
        CardDisplayInfo GetCardDisplayInfo(int id);
    }

   
    public class CardTable<T> : Table<CardProperties> where T: CardInfo { 
        public TableIndex<int, CardProperties> IDIndex { get; private set; }
        public TableIndex<CardType, CardProperties> TypeIndex { get; private set; }

        public TableIndex<Rarity, CardProperties> RarityIndex { get; private set; }
        public CardTable() {
            IDIndex = new TableIndex<int, CardProperties>(item => item.GetID());
            TypeIndex = new TableIndex<CardType, CardProperties>(item => item.CardType);
            RarityIndex = new TableIndex<Rarity, CardProperties>(item => item.rarity);
        }
        protected override void OnClear() {
            IDIndex.Clear();
            TypeIndex.Clear();
            RarityIndex.Clear();
        }

        public override void OnAdd(CardProperties item) {
            IDIndex.Add(item);
            TypeIndex.Add(item);
            RarityIndex.Add(item);
        }

        public override void OnRemove(CardProperties item) {
           IDIndex.Remove(item);
           TypeIndex.Remove(item);
           RarityIndex.Remove(item);
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
            resLoader.ReleaseAllAssets();
            RegisterCardInfo();
        }

        
        public void RegisterCardInfo() {
            //RegisterCharacterCardInfo(0, "TestCharacter",0, 100, 10,50, Rarity.Normal, CharacterType.Warrior, typeof(TestCharacter));
            // CardBasicProperties cardBasicPropertiesConfig = resLoader.LoadSync<CardBasicProperties>("card_properties", "CardProperties");
            CardBasicProperties cardBasicPropertiesConfig = Resources.Load<CardBasicProperties>("CardProperties");
            
            foreach (CardProperties cardProperties in cardBasicPropertiesConfig.CardDatas) {
                if (cardProperties.CardType == CardType.Character) {
                    RegisterCharacterCardInfo(cardProperties);
                }
                AllCardInfos.Add(cardProperties);
            }
        }

        public IEnumerable<CardProperties> GetCardProperties(Func<CardProperties, bool> contition) {
            return AllCardInfos.Get(properties => contition(properties) && properties.ID != "10000");
        }

        public CardDisplayInfo GetCardDisplayInfo(int id) {
            return AllCardInfos.GetByID(id).CardDisplayInfo;
        }

        public CardInfo GetNewCardInfoFromID(int id) {
            CardProperties cardAttributes = AllCardInfos.GetByID(id);
            return Activator.CreateInstance(cardAttributes.GetConcreteType(), cardAttributes) as CardInfo;
        }

        public static CardInfo GetNewCardInfoFromCardProperty(CardProperties properties) {
            return Activator.CreateInstance(properties.GetConcreteType(), properties) as CardInfo;
        }

        private void RegisterCharacterCardInfo(CardProperties cardProperties) {
            
            AllCharacterCardInfos.Add(cardProperties);
        }

    }
}
