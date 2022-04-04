using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework;
using MikroFramework.ActionKit;
using MikroFramework.Architecture;
using MikroFramework.Event;
using UnityEngine;
using EventType = MikroFramework.Event.EventType;

namespace MainGame
{
    public class BattleAnimationEventSystemUnregister : IUnRegister
    {
        public Dictionary<Type, List<MikroAction>> AnimationQueueRecord;
        public MikroAction action;
        public Type eventType;
        public Sequence sequence;

        public BattleAnimationEventSystemUnregister(Dictionary<Type, List<MikroAction>> animationQueue,
            Type eventType, MikroAction action, Sequence sequence) {

            this.AnimationQueueRecord = animationQueue;
            this.action = action;
            this.eventType = eventType;
            this.sequence = sequence;
        }

        public void UnRegister() {
            AnimationQueueRecord[eventType].Remove(action);
            sequence.RemoveAction(action);
            action.RecycleToCache();
        }

    }

    public class BattleEventSystemUnregister : IUnRegister {
        public Dictionary<Type, Action<IBattleEvent>> callbackDic;
        private Action<IBattleEvent> callback;
        public Type eventType;
       
        public BattleEventSystemUnregister(Dictionary<Type, Action<IBattleEvent>> callbackDict,
            Action<IBattleEvent> callback, Type eventType) {
            this.callbackDic = callbackDict;
            this.callback = callback;
            this.eventType = eventType;
        }

        public void UnRegister() {
            callbackDic[eventType] -= callback;
        }

    }
  
    public interface IBattleEventControlSystem : ISystem {
        
        public IUnRegister RegisterEffectToBattleEvent(Type eventType, Action<IBattleEvent> callback);

        public void RegisterAnimationToSequence(MikroAction callback);

        public void UnRegisterEffectFromBattleEvent(Type evenType, Action<IBattleEvent> callback);
       
    }
    public class BattleEventControlSystem : AbstractSystem, IBattleEventControlSystem {
        private Sequence sequence;

        public List<Type> BattleEventTypes = new List<Type>() {
            typeof(OnDrawCard),
            typeof(OnEnterBattleScene),
            typeof(OnCardDealt)
        };

        private Dictionary<Type, Action<IBattleEvent>> eventCallbackDic;
        

        protected override void OnInit() {
            eventCallbackDic = new Dictionary<Type, Action<IBattleEvent>>();
            
            
            ResetBattleEvent();

            this.RegisterEvent<IBattleEvent>(OnBattleEvent);
        }

        //battle end -> reset
        private void ResetBattleEvent() {
            eventCallbackDic.Clear();
            if (sequence != null) {
                sequence.RecycleToCache();
            }
            
            sequence = null;

            foreach (Type battleEventType in BattleEventTypes) {
                eventCallbackDic.Add(battleEventType, (e) => { });
            }
        }

        public IUnRegister RegisterEffectToBattleEvent(Type eventType, Action<IBattleEvent> callback) {
          
            eventCallbackDic[eventType] += callback;
           
            return new BattleEventSystemUnregister(eventCallbackDic, callback, eventType);
        }

        public void RegisterAnimationToSequence(MikroAction action) {
            Debug.Log($"Registering {action.GetType().ToString()} to the sequence");
            bool sequenceReady = !(sequence == null || sequence.Finished.Value || sequence.IsRecycled);
            Debug.Log(sequenceReady);
            if (!sequenceReady)
            {
                if (sequence != null) {
                    sequence.RecycleToCache();
                }
               
                sequence = Sequence.Allocate();

            }

            sequence.AddAction(action);

            if (!sequenceReady) {
                sequence.Execute();
            }
        }

        public void UnRegisterEffectFromBattleEvent(Type evenType, Action<IBattleEvent> callback) {
            Debug.Log(eventCallbackDic == null);
            if (eventCallbackDic != null) {
                eventCallbackDic[evenType] -= callback;
            }
           
        }

        
        private void OnBattleEvent(IBattleEvent e) {
            Debug.Log($"Event control: battle event {e.GetType().ToString()}");
            if (this.GetModel<IGameStateModel>().GameState.Value == GameState.Battle) {
                
                eventCallbackDic[e.GetType()]?.Invoke(e);
               
            }
           
        }

    }
}
