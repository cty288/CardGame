using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dreamteck;
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
            {CardType.Armor, 25},
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
               // Debug.Log($"Total enemy passed: {totalEnemyPassed}");
                UpdateCurrentRarityPossibility(totalEnemyPassed);
                LevelType levelType = ev.PreviousLevelType;
                
                //generate a couple of cards



                List<CardInfo> rewardCards = new List<CardInfo>();
                for (int i = 0; i < CardsPerReward; i++) {
                    Rarity rarity = GetRewardCardRarity(levelType);
                    CardType type = GetRewardCardType();
                    Debug.Log($"Passed enemy level Rarity: {rarity}");
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
                    else {
                        //proc gened cards
                        //1: get all effect belongs to that card type
                        //2: select one effect corresponding to the generated rarity
                        //3: generate the number of remaining effects (0-2)
                        //4: For the remaining effects, randomly select effects belongs to that card type, without repetition. But 
                        //    their rarity must be lower (or equal) to the generated rarity.
                        //5: SPECIAL for Attack cards: Attack cards can only have one Attack Effect. The remaining effects will be chosen
                        //    from Skill and Armor Effects; 
                        //    Attack and Skill can Use Armor Effects
                        //6: Note: Some cards are "Pointable", meaning they need to specify a target; the proc card generator
                        //    will make sure a card does not contain more than 2 pointable effects
                        bool hasPointable = false;
                        bool hasMulti = false;

                        List<EffectCommand> alreadyGeneratedEffects = new List<EffectCommand>();

                        //setp 1
                        EffectTable allEffectsOfType = this.GetModel<IEffectClassificationModel>().ConcatableEffectsByCardType[type];
                        List<EffectCommand> allEffectsOfTypeWithSelectedRarity = allEffectsOfType.ConcatableRarityIndex.Get((rar => 
                                    rar == rarity || rar == Rarity.Multi))
                            .Select(effect => effect as EffectCommand).ToList();
                        if (allEffectsOfTypeWithSelectedRarity.Any()) {
                            //step2
                            EffectCommand cmd =
                                allEffectsOfTypeWithSelectedRarity[
                                    ran.Next(0, allEffectsOfTypeWithSelectedRarity.Count)];
                          
                           
                            
                            hasPointable = cmd is IPointable;
                            hasMulti = cmd is IEffectContainer;
                            alreadyGeneratedEffects.Add(cmd);

                            Debug.Log($"Card Type {type} Count: " + allEffectsOfTypeWithSelectedRarity.Count + " " +
                                      $"Primary Pick: {cmd.GetType().Name}. Already Generated Generated Effects Count: {alreadyGeneratedEffects.Count}");
                        }

                        //step3
                        int remainingEffectsCount = ran.Next(1, 3);
                        List<EffectCommand> allRemainingEffects = new List<EffectCommand>();

                        //step4&5
                        if (type != CardType.Attack) {
                            allRemainingEffects.AddRange(allEffectsOfType.ConcatableRarityIndex
                                .Get(r => ((int) rarity >= (int) r) || r== Rarity.Multi).Select(effect => effect as EffectCommand).ToList());

                            if (type == CardType.Armor) { //add skills for armors
                                allRemainingEffects.AddRange(this.GetModel<IEffectClassificationModel>().ConcatableEffectsByCardType[CardType.Skill]
                                    .ConcatableRarityIndex.Get(r => ((int)rarity >= (int)r) || r == Rarity.Multi).Select(effect => effect as EffectCommand).ToList());
                            }
                        }else {

                            allRemainingEffects.AddRange( this.GetModel<IEffectClassificationModel>().ConcatableEffectsByCardType[CardType.Skill]
                                .ConcatableRarityIndex.Get(r => ((int)rarity >= (int)r) || r == Rarity.Multi).Select(effect => effect as EffectCommand).ToList()) ;
                        }

                        if (type == CardType.Attack || type == CardType.Skill) {
                            allRemainingEffects.AddRange(this.GetModel<IEffectClassificationModel>().ConcatableEffectsByCardType[CardType.Armor]
                                .ConcatableRarityIndex.Get(r => ((int)rarity >= (int)r) || r == Rarity.Multi).Select(effect => effect as EffectCommand).ToList()) ;
                        }

                        remainingEffectsCount = Mathf.Min(remainingEffectsCount, allRemainingEffects.Count);
                        Shuffle(allRemainingEffects);

                        for (int j = 0; j < remainingEffectsCount; j++) {
                            bool conditionSatisfy = false;


                            while (!conditionSatisfy) {
                                conditionSatisfy = true;


                                if (!alreadyGeneratedEffects.Contains(allRemainingEffects[0])) {
                                    if (allRemainingEffects[0] is IPointable) {
                                        if (hasPointable) {
                                            conditionSatisfy = false;
                                        }
                                    }

                                    if (allRemainingEffects[0] is IEffectContainer) {
                                        if (hasMulti) {
                                            conditionSatisfy = false;
                                        }
                                    }

                                    if (conditionSatisfy) {
                                        alreadyGeneratedEffects.Add(allRemainingEffects[0]);
                                        if (allRemainingEffects[0] is IPointable) {
                                            hasPointable = true;
                                        }
                                        if (allRemainingEffects[0] is IEffectContainer) {
                                            hasMulti = true;
                                        }
                                    }
                                }
                                else {
                                    conditionSatisfy = false;
                                }
                                allRemainingEffects.RemoveAt(0);
                                if (allRemainingEffects.Count == 0) {
                                    break;
                                }
                            }
                            

                            
                            if (allRemainingEffects.Count == 0) {
                                break;
                            }
                        }

                        //create proc genereted card instance, at effects to it
                        //Get Cost First; and create their instance by the way
                        int cost = 0;
                        List<EffectCommand> effectInstances = new List<EffectCommand>();
                        foreach (EffectCommand effect in alreadyGeneratedEffects) {
                            
                            IConcatableEffect effectInstance = effect.Clone() as IConcatableEffect;
                            if (effectInstance.Rarity == Rarity.Multi) {
                                effectInstance.Rarity = rarity;
                            }
                            effectInstance.OnGenerationPrep();
                            int cardCost = effectInstance.CostValue;
                            Debug.Log($"Effect Instance {effectInstance.GetType()} Cost Value: {cardCost}");
                            if (cost + cardCost > 10) {
                                effectInstance.RecycleToCache();
                                continue;
                            }

                            cost += effectInstance.CostValue;
                            effectInstances.Add(effectInstance as EffectCommand);
                            Debug.Log($"Effect instances added: {effectInstance.GetType().Name}");
                        }
                        //add some offsets
                        cost += ran.Next(-1, 2);
                        cost = Mathf.Clamp(cost, 0,10);


                        ProceduralNormalCard procCard = new ProceduralNormalCard(cost, rarity, type,
                            "proc_card_name_general",
                            effectInstances);
                        procCard.ID = 10000;
                        rewardCards.Add(procCard);

                        
                    }
                }

                this.SendEvent<OnCardRewardGenerated>(new OnCardRewardGenerated()
                    { RewardCard =  rewardCards});
            }
           
        }
        public void Shuffle<T>(IList<T> list)
        {
            Random ran = this.GetSystem<ISeedSystem>().RandomGeneratorRandom;
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = ran.Next(0, n + 1);
                list.Swap(k, n);
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
                    //Debug.Log(newCurrentRarityPossibility[valuePair.Key]);
                }
                currentRarityPossibility.Clear();
                currentRarityPossibility = newCurrentRarityPossibility;
            }
        }

        public BindableProperty<int> CardsPerReward { get; private set; } = new BindableProperty<int>();
    }
}
