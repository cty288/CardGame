using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.BindableProperty;
using UnityEngine;

namespace MainGame
{
    public interface IGameTimeModel : IModel {
        /// <summary>
        /// In Minutes
        /// </summary>
        public BindableProperty<int> GameStartTime { get; }
        /// <summary>
        /// Total minutes passed (in the game's context) since the start of the game. This does not include the game start time
        /// </summary>
        public BindableProperty<int> TotalMinutesPassed { get; }

        public BindableProperty<int> CurrentMinuteOfDay { get; }
        public BindableProperty<int> CurrentHourOfDay { get;}
        public BindableProperty<int> CurentDay { get;}
        public BindableProperty<float> TimeSpeed { get; }

        public void AddMinutes(int minuteAdded);
    }
    public class GameTimeModel : AbstractModel, IGameTimeModel , ICanRegisterAndLoadSavedData, ICanGetModel
    {
        protected override void OnInit() {
            GameStartTime = this.RegisterAndLoadFromSavedData("start_time", new BindableProperty<int>(
                this.GetModel<IGameTimeConfigModel>().GameStartTime));

            TotalMinutesPassed = this.RegisterAndLoadFromSavedData("minutes_passed", new BindableProperty<int>(0));
            
            TimeSpeed = this.RegisterAndLoadFromSavedData("time_speed", new BindableProperty<float>(
                this.GetModel<IGameTimeConfigModel>().TimeSpeed));

            UpdateCurrentMinutesHoursAndDays();
        }

        private void UpdateCurrentMinutesHoursAndDays() {
            CurrentMinuteOfDay.Value = ((GameStartTime + TotalMinutesPassed.Value) % 60);
            CurrentHourOfDay.Value = ((GameStartTime + TotalMinutesPassed.Value) % 1440) / 60;
            CurentDay.Value = ((GameStartTime + TotalMinutesPassed.Value) / 1440);
        }

        /// <summary>
        /// Add minutes to the time model (unscaled by timespeed). Actual minutes affected by time scale will be automatically calculated
        /// </summary>
        /// <param name="minuteAdded"></param>
        public void AddMinutes(int minuteAdded) {
            TotalMinutesPassed.Value += Mathf.RoundToInt(minuteAdded * this.GetModel<IGameTimeConfigModel>().TimeSpeed);
            UpdateCurrentMinutesHoursAndDays();
        }

        public BindableProperty<int> GameStartTime { get; private set; } = new BindableProperty<int>();
        public BindableProperty<int> TotalMinutesPassed { get; private set; }
        public BindableProperty<int> CurrentMinuteOfDay { get; private set; } = new BindableProperty<int>(0);
        public BindableProperty<int> CurrentHourOfDay { get; private set; } = new BindableProperty<int>(0);
        public BindableProperty<int> CurentDay { get; private set; } = new BindableProperty<int>(0);
        public BindableProperty<float> TimeSpeed { get; private set; }
    }
}
