using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

namespace MainGame
{
    public abstract class AbstractEffectAlwaysTriggeredByEvents : EffectCommand, IAlwaysTriggeredByOtherEvents {
        public abstract List<IBattleEvent> TriggeredBy { get; set; }
        public abstract List<EffectCommand> TriggeredEffects { get; set; }

        public override EffectCommand OnCloned()
        {
            ITriggeredByEventsWhenDealt cmd = OnClone();
            cmd.TriggeredEffects = new List<EffectCommand>();
            foreach (EffectCommand triggeredEffect in TriggeredEffects)
            {
                cmd.TriggeredEffects.Add(triggeredEffect.Clone());
            }

            return cmd as EffectCommand;
        }

        public abstract ITriggeredByEventsWhenDealt OnClone();

        public override string GetLocalizedTextWithoutBold()
        {
            string result = GetConditionLocalizedPrefixTextWithoutBold();

            foreach (EffectCommand triggeredEffect in TriggeredEffects)
            {
                result += Localization("Comma");
                result += triggeredEffect.GetLocalizedTextWithoutBold();
            }

            return result;
        }
        protected override void OnExecute()
        {
            foreach (EffectCommand triggeredEffect in TriggeredEffects)
            {
                triggeredEffect.Execute();
            }
        }
        public abstract string GetConditionLocalizedPrefixTextWithoutBold();
    }

    [ES3Serializable]
    public abstract class AbstractConcatableEffectAlwaysTriggerdByEvent : AbstractEffectAlwaysTriggeredByEvents,
        ITriggeredByEventsWhenDealt, IConcatableEffect
    {
        [ES3Serializable]
        public Rarity Rarity { get;  set; } = Rarity.Multi;

        public int CostValue
        {
            get
            {
                float effectCost = 0;
                foreach (EffectCommand triggeredEffect in TriggeredEffects)
                {
                    effectCost += (triggeredEffect as IConcatableEffect).CostValue;
                }

                return Mathf.RoundToInt(ConditionBaseCost + effectCost);
            }
        }
        public override EffectCommand OnCloned()
        {
            ITriggeredByEventsWhenDealt cmd = OnClone();
            cmd.TriggeredEffects = new List<EffectCommand>();
            foreach (EffectCommand triggeredEffect in TriggeredEffects)
            {
                cmd.TriggeredEffects.Add(triggeredEffect.Clone());
            }

            (cmd as IConcatableEffect).Rarity = Rarity;
            return cmd as EffectCommand;
        }
        public abstract float ConditionBaseCost { get; protected set; }
        public void OnGenerationPrep()
        {
            //add random effects
            //rarity changed by the reward system
            TriggeredEffects = new List<EffectCommand>();
            List<IConcatableEffect> allEffects = new List<IConcatableEffect>();
            IEffectClassificationModel model = this.GetModel<IEffectClassificationModel>();
            allEffects.AddRange(model.ConcatableEffectsByCardType[CardType.Skill].ConcatableRarityIndex.Get(Rarity));

            allEffects.AddRange(model.ConcatableEffectsByCardType[CardType.Character].ConcatableRarityIndex
                .Get(Rarity));

            IConcatableEffect pickedEffect =
                allEffects[this.GetSystem<ISeedSystem>().RandomGeneratorRandom.Next(0, allEffects.Count)];

            EffectCommand eff = (pickedEffect as EffectCommand).Clone();

            if (eff is IConcatableEffect effect)
            {
                effect.OnGenerationPrep();
            }
            else
            {
                Debug.LogError($"{eff} is not a concatable effect!");
            }

            TriggeredEffects.Add(pickedEffect as EffectCommand);
        }
    }
}
