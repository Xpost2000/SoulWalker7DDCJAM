using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class UIGameOverScript : MonoBehaviour
{
    public Button restart_button;
    public Button quit_button;
    public Button return_to_desktop_button;

    void Start() {
        restart_button.onClick.AddListener(OnRestart);
        quit_button.onClick.AddListener(OnQuit);
        return_to_desktop_button.onClick.AddListener(OnReturnToDesktop);
    }

    void OnEnable() {
        // begin my animation?
    }

    void OnRestart() {
        print("on restart");
        GameManagerScript.instance().Restart();
    }

    void OnQuit() {
        print("on quit");
        GameManagerScript.instance().Restart();
    }

    void OnReturnToDesktop() {
        print("On return to desktop?");
        GameManagerScript.instance().TryToKillGame();
    }

    void OnDisable() {} //???
}
