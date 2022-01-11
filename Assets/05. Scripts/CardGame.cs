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

            this.RegisterSystem<ISeedSystem>(new SeedSystem());
            this.RegisterSystem<ISaveSystem>(new SaveSystem());
            this.RegisterSystem<ITimeSystem>(new TimeSystem());
            this.RegisterSystem<IGameMapSystem>(new GameMapSystem());
            this.RegisterSystem<IGameTimeSystem>(new GameTimeSystem());
            //this.RegisterSystem<ISelectionSystem>(new SelectionSystem());
           
            
        }
    }
}
