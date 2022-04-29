using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using MikroFramework;
using TMPro;
using MikroFramework.ActionKit;
using System.Collections.Generic;

namespace MainGame {
    [ES3Serializable]
	public class BlessingRingArmor : CardInfo {
	public BlessingRingArmor () {}
	public BlessingRingArmor(CardProperties attributes) : base(attributes) {}
	public override List<MikroAction> InitialEffectOverrideAnimation { get; protected set; }
	public override List<EffectCommand> SetInitialEffects() {
        return new List<EffectCommand>() {
			new AddPropertyValueEffect(new Vector2Int(3,5))
        };
    }

	public override List<EffectCommand> SetInitialPrimitiveBuffEffects() {
        return new List<EffectCommand>() {

        };
    }

	 public override void OnCardDealtSuccess() {
		
	}
	}
}