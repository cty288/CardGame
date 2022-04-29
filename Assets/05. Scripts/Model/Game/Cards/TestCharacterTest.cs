using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using MikroFramework;
using TMPro;
using MikroFramework.ActionKit;
using System.Collections.Generic;

namespace MainGame {
	public class TestCharacterTest : CharacterCardInfo {
	public TestCharacterTest () {}
	public TestCharacterTest(CardProperties attributes) : base(attributes) {}
	public override List<MikroAction> InitialEffectOverrideAnimation { get; protected set; }
	public override List<EffectCommand> SetInitialEffects() {
		throw new System.NotImplementedException();
	}

	public override List<EffectCommand> SetInitialPrimitiveBuffEffects() {
		throw new System.NotImplementedException();
	}
	}
}