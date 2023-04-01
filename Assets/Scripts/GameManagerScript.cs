using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour {
    public bool interpolate_animations = true;

    // handle game state

    void Start() {
        
    }

    void Update() {
        
    }

    public static GameManagerScript instance() {
        var result = GameObject.Find("MainGameManager?");
        return result.GetComponent<GameManagerScript>();
    }
}
