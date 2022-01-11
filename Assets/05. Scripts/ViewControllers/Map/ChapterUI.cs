using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChapterUI : MonoBehaviour {
    private TMP_Text chapterText;

    private void Awake() {
        chapterText = GetComponent<TMP_Text>();
    }

    private void Start() {
        chapterText.text = $"Chapter: {GameManager.Singleton.Chapter}";
    }
}
