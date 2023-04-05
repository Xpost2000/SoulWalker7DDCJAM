using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreenScript : MonoBehaviour
{
    void Start() {}
    void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
        print("Loading screen registers self");
    }
    void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        print("Loading screen unregisters self");
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        print("Loading screen finishes. bye");
        GameManagerScript.instance().State = GameState.Ingame;
    }
}
