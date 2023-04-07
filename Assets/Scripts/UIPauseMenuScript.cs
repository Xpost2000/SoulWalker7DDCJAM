using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class UIPauseMenuScript : MonoBehaviour
{
    // Start is called before the first frame update
    public Button resume_button;
    public Button quit_button;
    public Button return_to_desktop_button;
    public Button options_button;

    void Start() {
        resume_button.onClick.AddListener(OnResume);
        quit_button.onClick.AddListener(OnQuit);
        options_button.onClick.AddListener(OnOptions);
        return_to_desktop_button.onClick.AddListener(OnReturnToDesktop);
    }

    void OnEnable() {
        // begin my animation?
    }

    void OnDisable() {} //???

    void OnResume() {
        print("on resume?");
        GameManagerScript.instance().State = GameState.Ingame;
    }

    void OnQuit() {
        print("on quit");
        GameManagerScript.instance().Restart();
    }

    void OnReturnToDesktop() {
        print("On return to desktop?");
        GameManagerScript.instance().TryToKillGame();
    }

    void OnOptions() {
        print("On options?");
        GameManagerScript.instance().State = GameState.Options;
    }
}
