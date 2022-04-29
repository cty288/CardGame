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
	public partial class GameStateCanvas : AbstractMikroController<CardGame> {
		[SerializeField] private GameObject ObjDeckViewPage;
		[SerializeField] private GameObject ObjViewLayout;
		[SerializeField] private Image ImgBar;
		[SerializeField] private TMP_Text TextGameTime;
		[SerializeField] private Image ImgSwitchSceneBG;
		[SerializeField] private TMP_Text TextGameState;
		[SerializeField] private Button BtnResetMap;
		[SerializeField] private Button BtnViewDeck;
	}
}