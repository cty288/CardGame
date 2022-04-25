using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.TimeSystem;
using UnityEngine;

namespace MainGame
{
    public class CardGame : Architecture<CardGame>
    {
       
        protected override void Init() {
            this.RegisterModel<IMapGenerationConfigModel>(new MapGenerationConfigModel());
            this.RegisterModel<IMapGenerationModel>(new MapGenerationModel());
            this.RegisterModel<IMapStateModel>(new MapStateModel());
           
            this.RegisterModel<IGameTimeConfigModel>(new GameTimeConfigModel());
            this.RegisterModel<IGameTimeModel>(new GameTimeModel());
            this.RegisterModel<IGameStateModel>(new GameStateModel());
            this.RegisterModel<IKeywordConfigModel>(new KeywordConfigModel());
            this.RegisterModel<ICardConfigModel>(new CardConfigModel());
            this.RegisterModel<IGameCardDeckModel>(new GameCardDeckModel());
            this.RegisterModel<IEffectClassificationModel>(new EffectClassificationModel());

            this.RegisterSystem<ISeedSystem>(new SeedSystem());
            this.RegisterSystem<ISaveSystem>(new SaveSystem());
            this.RegisterSystem<ITimeSystem>(new TimeSystem());
            this.RegisterSystem<IMapAnimationControlSystem>(new MapAnimationControlSystem());
            this.RegisterSystem<IGameMapControlSystem>(new GameMapControlSystem());
            this.RegisterSystem<IGameMapGenerationSystem>(new GameMapGenerationGenerationSystem());
            this.RegisterSystem<IGameTimeSystem>(new GameTimeSystem());
            this.RegisterSystem<IBattleCardDeckSystem>(new BattleCardDeckSystem());
            this.RegisterSystem<IBattleEventControlSystem>(new BattleEventControlSystem());
            this.RegisterSystem<IBattleSystem>(new BattleSystem());
            this.RegisterSystem<IRewardSystem>(new RewardSystem());
            //this.RegisterSystem<ISelectionSystem>(new SelectionSystem());
        }

        public override void OnReboot() {
            this.GetSystem<ITimeSystem>().Reset();
        }
    }
}
