using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MikroFramework.Architecture;
using MikroFramework.BindableProperty;
using UnityEngine;
using Random = System.Random;

namespace MainGame
{
    public interface IRewardSystem : ISystem {
        public BindableProperty<int> CardsPerReward { get; }
    }
    public class RewardSystem : AbstractSystem, IRewardSystem, ICanRegisterAndLoadSavedData {


        private Dictionary<Rarity, float> initialRarityPossibility = new Dictionary<Rarity, float>() {
            {Rarity.Normal, 80},
            {Rarity.Rare, 20},
            {Rarity.Epic, 0},
            {Rarity.Legend, 0}
        };

        private Dictionary<Rarity, float> rarityPossibilityGrothRatePerLevelProgress =
            new Dictionary<Rarity, float>() {
                {Rarity.Normal, -2.5f},
                {Rarity.Rare, 1f},
                {Rarity.Epic, 1.25f},
                {Rarity.Legend, 0.25f}
            };

        private Dictionary<Rarity, float> currentRarityPossibility = new Dictionary<Rarity, float>();


        private Dictionary<Rarity, float> RarityForElitePossibility = new Dictionary<Rarity, float>() {
            {Rarity.Epic, 70},
            {Rarity.Legend, 30}
        };

        /// <summary>
        /// For now, we will not deal with characters, because it requires extra things
        /// </summary>
        private Dictionary<CardType, float> cardTypeRarityPossibility = new Dictionary<CardType, float>() {
            {CardType.Character, 0},
            {CardType.Skill, 25},
            {CardType.Area, 25},
            {CardType.Armor, 0},
            {CardType.Attack, 25}
        };


        private int maxProgressToChangePossibility = 20;
        protected override void OnInit() {
            this.CardsPerReward = this.RegisterAndLoadFromSavedData("cards_per_reward", new BindableProperty<int>(3));
            this.RegisterEvent<IBattleEvent>(OnEnemyLevelPassed);
            foreach (KeyValuePair<Rarity, float> valuePair in initialRarityPossibility) {
                currentRarityPossibility.Add(valuePair.Key, valuePair.Value);
            }
        }

        private void OnEnemyLevelPassed(IBattleEvent e) {
            Random ran = this.GetSystem<ISeedSystem>().RandomGeneratorRandom;
            ICardConfigModel configModel = this.GetModel<ICardConfigModel>();
            if (e is OnEnemyLevelPassed ev) {
                //generate rewards
                int totalEnemyPassed = this.GetModel<IGameStateModel>().TotalEnemyPassed;
                Debug.Log($"Total enemy passed: {totalEnemyPassed}");
                UpdateCurrentRarityPossibility(totalEnemyPassed);
                LevelType levelType = ev.LevelVertex.Value.LevelType;
                //generate a couple of cards



                List<CardInfo> rewardCards = new List<CardInfo>();
                for (int i = 0; i < CardsPerReward; i++) {
                    Rarity rarity = GetRewardCardRarity(levelType);
                    CardType type = GetRewardCardType();

                    if (ran.Next(0, 100) <= 50) { //designed cards
                        IList<CardProperties> properties = configModel.GetCardProperties(cardProperties =>
                            cardProperties.rarity == rarity &&
                            cardProperties.CardType == type).ToList();

                        //unable to find such a card -> remove one condition
                       
                        if (!properties.Any()) {
                            properties = configModel.GetCardProperties(cardProperties =>
                                cardProperties.rarity == rarity && cardProperties.CardType!=CardType.Character).ToList();
                        }

                        if (properties.Count > 0) {
                            Debug.Log(properties.Count);
                            Debug.Log(properties[0].NameKey);
                            CardInfo generatedCard =
                                CardConfigModel.GetNewCardInfoFromCardProperty(properties[ran.Next(0, properties.Count)]);

                            rewardCards.Add(generatedCard);
                            Debug.Log($"Reward card add a generated card: {generatedCard.NameKey}");
                        }

                       
                    }
                    else {//proc gened cards

                    }
                }
            }
           
        }
        private CardType GetRewardCardType()
        {
            int random = this.GetSystem<ISeedSystem>().RandomGeneratorRandom.Next(0, 101);
            float progress = 0;
            foreach (KeyValuePair<CardType, float> valuePair in cardTypeRarityPossibility)
            {
                if (progress <= random && random < valuePair.Value + progress)
                {
                    return valuePair.Key;
                }
                progress += valuePair.Value;
            }

            return CardType.Skill;

        }

        private Rarity GetRewardCardRarity(LevelType levelType) {
            int random = this.GetSystem<ISeedSystem>().RandomGeneratorRandom.Next(0, 101);
            if (levelType == LevelType.Elite || levelType == LevelType.Boss) {
                if (random <= RarityForElitePossibility[Rarity.Epic]) {
                    return Rarity.Epic;
                }else {
                    return Rarity.Legend;
                }
            }
            else {
                float progress = 0;
                foreach (KeyValuePair<Rarity, float> valuePair in currentRarityPossibility) {
                    if (progress <= random && random < valuePair.Value + progress) {
                        return valuePair.Key;
                    }
                    progress += valuePair.Value;
                }

                return Rarity.Normal;
            }
        }

        private void UpdateCurrentRarityPossibility(int totalEnemyPassed) {
            if (totalEnemyPassed <= maxProgressToChangePossibility) {
                Dictionary<Rarity, float> newCurrentRarityPossibility = new Dictionary<Rarity, float>();

                foreach (KeyValuePair<Rarity, float> valuePair in currentRarityPossibility) {
                    float growthRate = rarityPossibilityGrothRatePerLevelProgress[valuePair.Key];
                    newCurrentRarityPossibility.Add(valuePair.Key, initialRarityPossibility[valuePair.Key] + growthRate * totalEnemyPassed);
                    Debug.Log(newCurrentRarityPossibility[valuePair.Key]);
                }
                currentRarityPossibility.Clear();
                currentRarityPossibility = newCurrentRarityPossibility;
            }
        }

        public BindableProperty<int> CardsPerReward { get; private set; } = new BindableProperty<int>();
    }
}
