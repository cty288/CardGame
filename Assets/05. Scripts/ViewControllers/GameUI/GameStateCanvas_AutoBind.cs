using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using MikroFramework;
using TMPro;
using MikroFramework.Architecture;

namespace MainGame {
	public partial class GameStateCanvas : AbstractMikroController<CardGame> {
		[SerializeField] private Image ImgBar;
		[SerializeField] private TMP_Text TextGameTime;
		[SerializeField] private Image ImgSwitchSceneBG;
	}
}