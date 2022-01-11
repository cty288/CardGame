using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SeedUI : MonoBehaviour
{
  
    void Start() {
        GetComponent<TMP_Text>().text = $"Game Seed: {GameManager.Singleton.GameSeed}\n" +
                                        $"Chapter Seed: {GameManager.Singleton.LevelSeed}";
    }

   
}
