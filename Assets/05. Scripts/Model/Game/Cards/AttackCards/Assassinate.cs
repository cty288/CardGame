using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using MikroFramework;
using TMPro;
using MikroFramework.ActionKit;
using System.Collections.Generic;

namespace MainGame {
	public class Assassinate : CardInfo {
	public Assassinate () {}
	public Assassinate(CardProperties attributes) : base(attributes) {}
	public override List<MikroAction> InitialEffectOverrideAnimation { get; protected set; }
	public override List<EffectCommand> SetInitialEffects() {
		return new List<EffectCommand>() {
			new AttackMultipleTimesAndKillRandomEnemyEffect(1)
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