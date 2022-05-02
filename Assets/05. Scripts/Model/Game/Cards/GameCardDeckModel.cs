using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.BindableProperty;
using UnityEngine;

namespace MainGame
{
    public interface IGameCardDeckModel : IModel {
        public BindableProperty<List<CharacterCardInfo>> CharactersInDeck { get; }
        public BindableProperty<List<CharacterCardInfo>> DeadCharacters { get; }
        
        /// <summary>
        /// Does not include characters
        /// </summary>
        public BindableProperty<List<CardInfo>> CardsInDeck { get; }

       
    }
    /// <summary>
    /// Not Battle Deck. This Model charges all cards that the player holds during the game
    /// </summary>
    public class GameCardDeckModel : AbstractModel, IGameCardDeckModel, ICanGetModel, ICanRegisterAndLoadSavedData {
        private ICardConfigModel cardConfigModel;
        protected override void OnInit() {
            cardConfigModel = this.GetModel<ICardConfigModel>();

            CharactersInDeck.Value = this.RegisterAndLoadFromSavedData("characters_alive", new List<CharacterCardInfo>() {
                cardConfigModel.GetNewCardInfoFromID(0) as CharacterCardInfo,
                cardConfigModel.GetNewCardInfoFromID(0) as CharacterCardInfo
            });
            DeadCharacters.Value = this.RegisterAndLoadFromSavedData("characters_dead", new List<CharacterCardInfo>() {
                
            });
            CardsInDeck.Value = this.RegisterAndLoadFromSavedData("cards_deck", new List<CardInfo>() {
                cardConfigModel.GetNewCardInfoFromID(1),
                cardConfigModel.GetNewCardInfoFromID(2),
                cardConfigModel.GetNewCardInfoFromID(3),
                cardConfigModel.GetNewCardInfoFromID(4),
                cardConfigModel.GetNewCardInfoFromID(5),
                cardConfigModel.GetNewCardInfoFromID(6)
            });
        }

        public BindableProperty<List<CharacterCardInfo>> CharactersInDeck { get; } =
            new BindableProperty<List<CharacterCardInfo>>();
        public BindableProperty<List<CharacterCardInfo>> DeadCharacters { get; } =
            new BindableProperty<List<CharacterCardInfo>>();
        public BindableProperty<List<CardInfo>> CardsInDeck { get; } = new BindableProperty<List<CardInfo>>();
    }
}
