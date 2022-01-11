using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = System.Random;

public class GameManager : MonoBehaviour {
    public static GameManager Singleton;

    [Header("Random Related Stuff")]
   

    public Random random;

    private TMP_InputField inputField;
    private Button restartRandomButton;
    private Button restartWithSeedButton;

    private Button nextChapterButton;

    [HideInInspector]
    public int GameSeed;

    [HideInInspector] 
    public int LevelSeed;

    public int Chapter = 1;


    private void Awake() {
        if (Singleton != null) {
            Destroy(this.gameObject);
        }
        else {
            Singleton = this;
            DontDestroyOnLoad(this.gameObject);
        }

        SetRandomSeed();
    }

    

    private void Update() {
        if (!restartRandomButton) {
            restartRandomButton = GameObject.Find("RestartRandomButton").GetComponent<Button>();
            restartWithSeedButton = GameObject.Find("RestartWithSeed").GetComponent<Button>();
            inputField = GameObject.Find("SeedInput").GetComponent<TMP_InputField>();
            nextChapterButton = GameObject.Find("NextChapterButtonParent").transform.Find("NextChapterButton")
                .GetComponent<Button>();

            restartRandomButton.onClick.AddListener(RestartGameRandom);
            restartWithSeedButton.onClick.AddListener(RestartGameWithSeed);
            nextChapterButton.onClick.AddListener(GoToNextChapter);
        }
    }

    private void GoToNextChapter() {
        Chapter++;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        LevelSeed = random.Next(-1000000000, 1000000000);
    }

    private void SetRandomSeed() {
        GameSeed = UnityEngine.Random.Range(-1000000000, 1000000000);
        random = new Random(GameSeed);
        LevelSeed = random.Next(-1000000000, 1000000000);
    }

    public void RestartGameRandom() {
        SetRandomSeed();
        ResetInfo();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void RestartGameWithSeed() {
        int.TryParse(inputField.text,out GameSeed);
        random = new Random(GameSeed);
        ResetInfo();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        LevelSeed = random.Next(-1000000000, 1000000000);
    }

    public void BossLevelPass() {
        nextChapterButton.gameObject.SetActive(true);
    }

    void ResetInfo() {
        Chapter = 1;
    }
}
