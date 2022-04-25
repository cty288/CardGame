using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

namespace MainGame
{
    public interface IRewardSystem : ISystem {

    }
    public class RewardSystem : AbstractSystem, IRewardSystem {
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
            this.RegisterEvent<OnEnemyLevelPassed>(OnEnemyLevelPassed);
        }

        private void OnEnemyLevelPassed(OnEnemyLevelPassed e) {
            //generate rewards
            
        }
    }
}
