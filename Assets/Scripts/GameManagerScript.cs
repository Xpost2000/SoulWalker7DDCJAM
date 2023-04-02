using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState {
    None,
    MainMenu,
    Ingame,
    Pause,
    GameOver,
    GameWin
}

public class GameManagerScript : MonoBehaviour {
    public bool interpolate_animations = true;
    public int target_framerate = 20; // crunchy framerate.

    // handle game state
    public GameObject player;
    public GameObject user_interface_container;

    private GameState m_state = GameState.None;
    public GameState State {
        get { return m_state; }
        set {
            if (value != m_state) {
                m_state = value;

                // enable the appropriate objects
                // based off the game state.
                print(m_state);
                switch (m_state) {
                    case GameState.GameWin: {
                    } break;
                    case GameState.GameOver: {
                        foreach (Transform child in user_interface_container.transform) {
                            child.gameObject.SetActive(false);
                        }

                        GameObject gameover_ui = user_interface_container.transform.Find("UIGameoverCanvas").gameObject;
                        gameover_ui.SetActive(true);

                        player.GetComponent<PlayerController>().DisableInput();
                    } break;
                    case GameState.Pause: {
                    } break;
                    case GameState.Ingame: {
                        foreach (Transform child in user_interface_container.transform) {
                            child.gameObject.SetActive(false);
                        }

                        GameObject gameplay_ui = user_interface_container.transform.Find("UIGameplayCanvas").gameObject;
                        gameplay_ui.SetActive(true);
                        player.GetComponent<PlayerController>().EnableInput();
                    } break;
                    case GameState.MainMenu: {
                    } break;
                    default: {
                        print("should this be possible?");
                    } break;
                }
            }
        }
    }

    void Start() {
        print("Hi, I begin");
        State = GameState.GameOver;
        // State = GameState.Ingame;

        Application.targetFrameRate = target_framerate;
    }

    public void TryToKillGame() {
#if UNITY_STANDALONE
        Application.Quit();
#endif
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    void Update() {}

    void LoadLevel() {
        // TODO: not tested!
    }

    public static GameManagerScript instance() {
        var result = GameObject.Find("MainGameManager?");
        return result.GetComponent<GameManagerScript>();
    }
}
