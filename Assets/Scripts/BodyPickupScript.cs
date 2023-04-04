using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
  This is the main feature of this game.

  Picking up various bodies to use as "armor",
  and shielding you from having your spirit exposed.

  Each body has it's own health amount.

  These are mostly data holders.

  Don't know how to use scriptable objects yet.
*/
public class BodyPickupScript : MonoBehaviour
{
    public int max_health = 20;
    public int health = -1;

    void Start() {
        if (health == -1) health = max_health;
    }

    void Update() {
        
    }

    void OnTriggerEnter(Collider collider) {
        // ?
    }

    void OnTriggerExit(Collider collider) {
        // ?
    }

    public string PromptString() {
        return "enter body form (" + health.ToString() + "/" + max_health.ToString() + ")?";
    }
}
