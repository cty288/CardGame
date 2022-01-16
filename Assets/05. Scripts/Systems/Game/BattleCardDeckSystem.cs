using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.BindableProperty;
using UnityEngine;

namespace MainGame
{
    public interface IBattleCardDeckSystem : ISystem {
        public BindableProperty<List<CardInfo>> CharactersInDeck { get; }
        public BindableProperty<List<CardInfo>> CardsInDeck { get; }
        public BindableProperty<List<CardInfo>> CardsInDiscardDeck { get; }
        public BindableProperty<List<CardInfo>> CardsInHand { get; }
        void ShuffleCards();
        void DrawAllCharacterCards();

        CharacterCardInfo DrawCardFromCharacterDeck();
        CardInfo DrawCardFromCardDeck();
    }
    public class BattleCardDeckSystem : AbstractSystem, IBattleCardDeckSystem {
        private IGameCardDeckModel gameCardDeckModel;

        public BindableProperty<List<CardInfo>> CharactersInDeck { get; private set; } =
            new BindableProperty<List<CardInfo>>();
        public BindableProperty<List<CardInfo>> CardsInDeck { get; private set; } =
            new BindableProperty<List<CardInfo>>();
        public BindableProperty<List<CardInfo>> CardsInDiscardDeck { get; private set; } =
            new BindableProperty<List<CardInfo>>();
        public BindableProperty<List<CardInfo>> CardsInHand { get; private set; } =
            new BindableProperty<List<CardInfo>>();

        protected override void OnInit() {
            gameCardDeckModel = this.GetModel<IGameCardDeckModel>();
            CharactersInDeck.Value = new List<CardInfo>();
            CharactersInDeck.Value.AddRange(gameCardDeckModel.CharactersInDeck.Value);

            CardsInDeck.Value = new List<CardInfo>();
            CardsInDeck.Value.AddRange(gameCardDeckModel.CardsInDeck.Value);

            CardsInDiscardDeck.Value = new List<CardInfo>();
            CardsInHand.Value = new List<CardInfo>();
        }

       

        public void ShuffleCards() {
            CharactersInDeck.Value = RandomSort(CharactersInDeck.Value);
        }

        private List<T> RandomSort<T>(List<T> list) {
            var random = this.GetSystem<ISeedSystem>().GameRandom;
            var newList = new List<T>();
            foreach (var item in list)
            {
                newList.Insert(random.Next(newList.Count), item);
            }
            return newList;
        }

        public void DrawAllCharacterCards() {
            int count = CharactersInDeck.Value.Count;
            for (int i = 0; i < count; i++) {
                DrawCardFromCharacterDeck();
            }
        }

        public CharacterCardInfo DrawCardFromCharacterDeck() {
            return (CharacterCardInfo) DrawCardFromDeck(CharactersInDeck);
        }

        public CardInfo DrawCardFromCardDeck() {
            return DrawCardFromDeck(CardsInDeck);
        }

        private CardInfo DrawCardFromDeck(BindableProperty<List<CardInfo>> deck) {
            //TODO: all cards drawn -> shuffle discard back to deck
            CardInfo cardInfo = deck.Value[0]; 
            deck.Value.RemoveAt(0);
            //Trigger Event
            this.SendEvent<IBattleEvent>(new OnDrawCard(){cardDraw = cardInfo});
            CardsInHand.Value.Add(cardInfo);
            return cardInfo;
        }
    }
}
