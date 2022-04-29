using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using MikroFramework;
using TMPro;
using MikroFramework.ActionKit;
using System.Collections.Generic;
using MikroFramework.Architecture;

namespace MainGame {
	public partial class DeckViewPage : AbstractMikroController<CardGame> {
		[SerializeField] private GameObject ObjViewLayout;
	}
}