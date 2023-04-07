using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

/*
  This technically refers to UI state but okay.
 */
public enum GameState {
    None,
    MainMenu,
    Ingame,
    Pause,
    GameOver,
    Options,
    Loading,
    GameWin
}

public class GameManagerScript : MonoBehaviour {
    public bool interpolate_animations = true;
    public int target_framerate = 20; // crunchy framerate.

    // prefabs
    public GameObject body_pickup_prefab;
    public GameObject prefab_projectile_tracer;

    // sound stuff
    public AudioSource playersoundsource; // lolol
    public AudioClip sound_step;
    public AudioClip hit_sound;
    public AudioClip fire_sound;
    public AudioClip teleport_sound;
    public AudioClip pickup_sound;

    // handle game state
    public GameObject player;
    public GameObject user_interface_container;
    public GameObject message_log;

    public GameObject ui_pause;
    public GameObject ui_gameover;
    public GameObject ui_mainmenu;
    public GameObject ui_ingame;
    public GameObject ui_win;
    public GameObject ui_loading;
    public GameObject ui_options;

    public string first_scene;

    public float seconds_per_turn = 3.5f;
    public int   turns_passed = 0;
    private float current_turn_timer = 0.0f;

    /*
      Game Actors that need to think on a turn basis
      should register to these callbacks on their own.

      Remember to unsubscribe or something.
    */
    public delegate void OnTurnStart();
    public delegate void OnTurnEnd();

    public event OnTurnStart on_turn_start;
    public event OnTurnEnd   on_turn_end;

    public void UpdateTurnTimer(float dt) {
        if (State == GameState.Ingame) {
            if (current_turn_timer >= seconds_per_turn) {
                InvokeNextTurn();
            } else {
                current_turn_timer += dt;
            }
        }
    }

    public void EnablePrompt(string settext) {
        ui_ingame.GetComponent<UIGameplayScript>().EnablePrompt(settext);
    }

    public void DisablePrompt() {
        ui_ingame.GetComponent<UIGameplayScript>().DisablePrompt();
    }

    public void InvokeNextTurn() {
        on_turn_end?.Invoke();
        current_turn_timer = 0;
        turns_passed += 1;
        on_turn_start?.Invoke();

        // just to make sure stuff works.
        MessageLog.NewMessage(
            "A turn has passed...",
            Color.yellow
        );
    }


    public MessageLogPanel MessageLog {
        get {
            return message_log.GetComponent<MessageLogPanel>();
        }
    }

    private GameState m_state = GameState.None;
    private GameState m_last_state = GameState.None;
    public GameState LastState {
        get {
            return m_last_state;
        }
    }

    public GameState State {
        get { return m_state; }
        set {
            if (value != m_state) {
                m_last_state = m_state;
                m_state = value;

                // enable the appropriate objects
                // based off the game state.
                print(m_state);
                switch (m_state) {
                    case GameState.Options: {
                        HideAllUIChildren();
                        ui_options.SetActive(true);
                        player.GetComponent<PlayerController>().DisableInput();
                    } break;
                    case GameState.Loading: {
                        HideAllUIChildren();
                        ui_loading.SetActive(true);
                        player.GetComponent<PlayerController>().DisableInput();
                    } break;
                    case GameState.GameWin: {
                        HideAllUIChildren();
                        ui_win.SetActive(true);
                        player.GetComponent<PlayerController>().DisableInput();
                    } break;
                    case GameState.GameOver: {
                        HideAllUIChildren();
                        ui_gameover.SetActive(true);
                        player.GetComponent<PlayerController>().DisableInput();
                    } break;
                    case GameState.Pause: {
                        HideAllUIChildren();
                        ui_pause.SetActive(true);
                        player.GetComponent<PlayerController>().DisableInput();
                    } break;
                    case GameState.Ingame: {
                        HideAllUIChildren();
                        ui_ingame.SetActive(true);
                        player.GetComponent<PlayerController>().EnableInput();
                    } break;
                    case GameState.MainMenu: {
                        HideAllUIChildren();
                        ui_mainmenu.SetActive(true);
                        player.GetComponent<PlayerController>().DisableInput();
                    } break;
                    default: {
                        print("should this be possible?");
                    } break;
                }
            }
        }
    }

    GameObject FindUI(string name) {
        return user_interface_container.transform.Find(name).gameObject;
    }

    public void OpenExpositionTutorialPanel(string message, float read_speed=0.05f, float delaytime=1.0f) {
        ui_ingame.GetComponent<UIGameplayScript>().OpenExpositionTutorialPanel(
            message, read_speed, delaytime
        );
    }

    void HideAllUIChildren() {
        foreach (Transform child in user_interface_container.transform) {
            child.gameObject.SetActive(false);
        }
    }

    public void LoadFirstLevel() {
        LoadLevel(first_scene);
    }

    void Start() {
        print("Hi, I begin");
        // State = GameState.GameOver;
        State = GameState.MainMenu;
        // State = GameState.GameWin;

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

    void Update() {
        float dt = Time.deltaTime;
        UpdateTurnTimer(dt);
    }
    void FixedUpdate() {
        // float dt = Time.deltaTime;
        // UpdateTurnTimer(dt);
    }

    public void LoadLevel(string scene_name) {
        // TODO: not tested!
        var existing = SceneManager.GetSceneByName(scene_name);
        if (existing == null || existing.isLoaded == false) {
            SceneManager.LoadScene(scene_name, LoadSceneMode.Additive);
        } else {
            print("NOTE: scene already loaded!");
        }
        State = GameState.Loading;
    }

    public void Restart() {
        SceneManager.LoadScene("MainGameScene", LoadSceneMode.Single);
    }

    public static GameManagerScript instance() {
        var result = GameObject.Find("MainGameManager?");
        return result.GetComponent<GameManagerScript>();
    }
}
