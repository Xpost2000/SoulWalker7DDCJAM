using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportingBallLevel3Script : MonoBehaviour
{
    public GameObject whereto;
    public bool do_not_teleport = false;
    // Start is called before the first frame update
    void Start() {
        
    }

    void OnTriggerEnter(Collider collider) {
        if (do_not_teleport) {
            return;
        }

        var gameobject = collider.gameObject;
        var controllercomponent = gameobject.GetComponent<GenericActorController>();
        if (controllercomponent) {
            controllercomponent.SetLogicalPosition(whereto.transform.position);
        } else {
            gameobject.transform.position = whereto.transform.position;
        }

        var teleportcomponent = whereto.GetComponent<TeleportingBallLevel3Script>();
        if (teleportcomponent) {
            // prevent teleporting instantly...
            teleportcomponent.do_not_teleport = true;
        }
    }
    void OnTriggerExit(Collider collider) {
        do_not_teleport = false;
    }

    // Update is called once per frame
    void Update() {
        
    }
}
