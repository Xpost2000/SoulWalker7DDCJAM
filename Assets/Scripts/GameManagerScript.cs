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

    // handle game state
    public GameObject player;
    public GameObject user_interface_container;
    public GameObject active_level_container;

    private GameState m_state = GameState.None;
    public GameState State {
        get { return m_state; }
        set {
            if (value != m_state) {
                m_state = value;

                // enable the appropriate objects
                // based off the game state.
                switch (m_state) {
                    default: {
                        print("should this be possible?");
                    } break;
                }
            }
        }
    }

    void Start() {
        State = GameState.Ingame;
    }

    void Update() {}

    public static GameManagerScript instance() {
        var result = GameObject.Find("MainGameManager?");
        return result.GetComponent<GameManagerScript>();
    }
}
