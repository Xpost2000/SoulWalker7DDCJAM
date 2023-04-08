using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalorLordDeath : MonoBehaviour
{
    // Start is called before the first frame update
    public GenericActorController my_controller;
    public GameObject teleport_point;
    public GenericActorController player;

    void Start() {
        player = GameManagerScript.instance().player.GetComponent<GenericActorController>();
        my_controller.on_death += OnDeath;
    }

    void OnDeath(ActorState death_form) {
        player.SetLogicalPosition(
            teleport_point.transform.position
        );
    }

    // Update is called once per frame
    void Update() {
        
    }
}
