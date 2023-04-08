using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelProgressorScript : MonoBehaviour
{
    // Start is called before the first frame update
    public string target = "$endgame$";
    void Start() {
        GetComponent<MeshRenderer>().enabled = false;
    }

    void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.tag == "Player") {
            print("Hi the progressor was violated!");
            print("I will teleport us to: " + target);

            switch (target) {
                case "$endgame$": {
                    GameManagerScript.instance().State = GameState.GameWin;
                } break;
                default: {
                    // this should be the level the progressor is in...
                    Scene current_scene = gameObject.scene;
                    GameManagerScript.instance().LoadLevel(target);
                    SceneManager.UnloadSceneAsync(current_scene);
                    print("Bye!");
                } break;
            }
        }
    }

    // Update is called once per frame
    void Update() {
        
    }
}
