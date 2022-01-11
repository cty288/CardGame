using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using MikroFramework;
using TMPro;
using MikroFramework.Architecture;
using MikroFramework.Event;

namespace MainGame {
	public partial class GameStateCanvas : AbstractMikroController<CardGame> {
        [SerializeField] private float minuteRollTime = 2f;
        private Tween minuteDisplayTween, hourDisplayTween;

        private int dayDisplay = 0;
        private int hourDisplay = 0;
        private int minuteDisplay = 0;

        private Action onMinuteChangeDone;

        private void Start() {
            this.GetModel<IGameTimeModel>().CurrentMinuteOfDay.RegisterWithInitValue(OnMinuteChange)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            this.GetModel<IGameTimeModel>().CurrentHourOfDay.RegisterWithInitValue(OnHourChange)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            this.GetModel<IGameTimeModel>().CurentDay.RegisterWithInitValue(OnDayChange)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void Update() {
            string hourDisplayText = hourDisplay < 10 ? $"0{hourDisplay}" : hourDisplay.ToString();
            string minuteDisplayText = minuteDisplay < 10 ? $"0{minuteDisplay}" : minuteDisplay.ToString();
            TextGameTime.text = $"Day {dayDisplay} - {hourDisplayText}:{minuteDisplayText}";
        }

        private void OnDayChange(int prevDay, int curDay) {
            dayDisplay = curDay;
        }

        private void OnHourChange(int prevHour, int curHour) {
            if (minuteDisplayTween != null && minuteDisplayTween.active) {
                onMinuteChangeDone += ()=>{ChangeHour(curHour);};
            }
            else {
                ChangeHour(curHour);
            }
            
        }

        private void ChangeHour(int curHour) {
            if (hourDisplayTween != null && hourDisplayTween.active)
            {
                hourDisplayTween.Kill();
            }
            if (hourDisplay > curHour)
            {
                int totalHour = 24 - hourDisplay + curHour;
                float speed1 = (float)(24 - hourDisplay) / totalHour;
                float speed2 = 1 - speed1;

                hourDisplayTween = DOTween.To(() => hourDisplay, x => hourDisplay = x, 23, 0.5f * speed1)
                    .OnComplete(() => {
                        hourDisplay = 0;
                        DOTween.To(() => hourDisplay, x => hourDisplay = x, curHour, 0.5f * speed2);
                    });

            }
            else
            {
                hourDisplayTween = DOTween.To(() => hourDisplay, x => hourDisplay = x, curHour, 0.5f);

            }

            onMinuteChangeDone = null;
        }

        private void OnMinuteChange(int prevMinute, int curMinute) {
            if (minuteDisplayTween != null && minuteDisplayTween.active)
            {
                minuteDisplayTween.Kill();
            }

            if (minuteDisplay > curMinute) {
               

                int totalMin = 60 - minuteDisplay + curMinute;
                float speed1 = (float) (60 - minuteDisplay) / totalMin;
                float speed2 = 1 - speed1;
                
                minuteDisplayTween = DOTween.To(() => minuteDisplay, x => minuteDisplay = x, 59, minuteRollTime * speed1)
                    .OnComplete(() => {
                        minuteDisplay = 0;
                        onMinuteChangeDone?.Invoke();
                        DOTween.To(() => minuteDisplay, x => minuteDisplay = x, curMinute, minuteRollTime * speed2).SetEase(Ease.Linear);
                    }).SetEase(Ease.Linear);

            }
            else {
                minuteDisplayTween = DOTween.To(() => minuteDisplay, x => minuteDisplay = x, curMinute, minuteRollTime)
                    .OnComplete(
                        () => {
                            onMinuteChangeDone?.Invoke();
                        }).SetEase(Ease.Linear);

            }
        }
    }
}