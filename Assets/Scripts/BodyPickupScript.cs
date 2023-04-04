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
    public string description = "tough body";
    public int max_health = 20;
    int health;

    void Start() {
        
    }

    void Update() {
        
    }

    void OnTriggerEnter(Collider collider) {
        // ?
    }

    void OnTriggerExit(Collider collider) {
        // ?
    }
}
