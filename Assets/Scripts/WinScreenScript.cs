using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinScreenScript : MonoBehaviour
{
    public float linger_time = 5;
    float linger_time_timer = 0;
    // Start is called before the first frame update
    void Start() {
        
    }

    void OnEnable() {
        linger_time_timer = linger_time;
    }

    // Update is called once per frame
    void Update() {
        linger_time_timer -= Time.deltaTime;
        if (linger_time_timer <= 0.0) {
            GameManagerScript.instance().Restart();
        }
    }
}
