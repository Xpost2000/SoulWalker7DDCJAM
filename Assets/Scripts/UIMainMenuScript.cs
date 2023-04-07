using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class UIMainMenuScript : MonoBehaviour
{
    public Button start_button;
    public Button quit_button;
    public Button return_to_desktop_button;
    public Button options_button;

    void Start() {
        start_button.onClick.AddListener(OnStart);
        quit_button.onClick.AddListener(OnQuit);
        options_button.onClick.AddListener(OnOptions);
        return_to_desktop_button.onClick.AddListener(OnReturnToDesktop);
    }

    // Update is called once per frame
    void Update() {
        
    }

    void OnStart() {
        print("on start?");
        GameManagerScript.instance().LoadFirstLevel();
    }

    void OnQuit() {
        print("on quit");
        // should animate?
        GameManagerScript.instance().TryToKillGame();
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
