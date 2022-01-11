using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

namespace MainGame
{
    public interface IGameTimeSystem : ISystem {

    }
    public class GameTimeSystem : AbstractSystem, IGameTimeSystem
    {
        protected override void OnInit() {
            this.RegisterEvent<IGameTimeUpdateEvent>(OnGameTimeUpdate);
        }

        private void OnGameTimeUpdate(IGameTimeUpdateEvent e) {
            Debug.Log($"Time Passed: {e.TimePassed}");
            this.GetModel<IGameTimeModel>().AddMinutes(e.TimePassed);
        }
    }
}
