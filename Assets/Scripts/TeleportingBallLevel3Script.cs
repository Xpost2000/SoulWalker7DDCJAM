using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportingBallLevel3Script : MonoBehaviour
{
    public GameObject whereto;

    // hack.
    public int do_not_teleport_for_n_frames = 0;
    // Start is called before the first frame update
    void Start() {
        
    }

    void OnTriggerEnter(Collider collider) {
        if (do_not_teleport_for_n_frames > 0) {
            print("I should not be teleporting!");
            return;
        }

        print(transform.parent.name);
        print("hi, teleporting");

        var teleportcomponent = whereto.transform.Find("teleballmesh").GetComponent<TeleportingBallLevel3Script>();
        if (teleportcomponent) {
            // prevent teleporting instantly...
            teleportcomponent.do_not_teleport_for_n_frames = 15;
            print("Hi, I set the other things' teleport flag to off!");
        }

        var gameobject = collider.gameObject;
        var controllercomponent = gameobject.GetComponent<GenericActorController>();
        if (controllercomponent) {
            controllercomponent.SetLogicalPosition(whereto.transform.position);
        }
        gameobject.transform.position = whereto.transform.position;
    }
    void OnTriggerExit(Collider collider) {
        print(transform.parent.name);
        print("hi, being left, so untele");
    }

    // Update is called once per frame
    void Update() {
        do_not_teleport_for_n_frames -= 1;
    }
}
