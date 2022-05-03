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
	public partial class ShouGeGe : AbstractMikroController<CardGame> {
		[SerializeField] private Image ImgName;
		[SerializeField] private TMP_Text TextAge;
	}
}