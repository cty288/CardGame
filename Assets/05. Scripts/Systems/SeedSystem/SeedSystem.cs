using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using MikroFramework.Architecture;
using MikroFramework.BindableProperty;
using MikroFramework.Serializer;
using UnityEngine;
using Random = System.Random;

namespace MainGame
{
    public interface ISeedSystem : ISystem {
        public Random GeneralRandom { get; }
        public Random MapRandom { get; }
        public Random GameRandom { get; }

        BindableProperty<int> Seed { get; }
       
    }
    public class SeedSystem : AbstractSystem, ISeedSystem, ICanRegisterAndLoadSavedData
    {
        protected override void OnInit() {
            Seed = this.RegisterAndLoadFromSavedData("game_seed", new BindableProperty<int>(UnityEngine.Random.Range(-2100000000, 2100000000)));
            GeneralRandom = this.RegisterAndLoadFromSavedData("general_random", new Random(Seed), SaveType.Binary);
            MapRandom = this.RegisterAndLoadFromSavedData("map_random",
                new Random(GeneralRandom.Next(-2100000000, 2100000000)), SaveType.Binary);
            GameRandom = this.RegisterAndLoadFromSavedData("game_random",
                new Random(GeneralRandom.Next(-2100000000, 2100000000)), SaveType.Binary);
           
        }

        public Random GeneralRandom { get; set; }
        public Random MapRandom { get; set; } = new Random();
        public Random GameRandom { get; set; }
        public BindableProperty<int> Seed { get; set; }
       
    } 
}
