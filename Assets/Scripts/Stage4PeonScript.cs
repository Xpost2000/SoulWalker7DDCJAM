using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage4PeonScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject balor_lord;
    public GenericActorController my_controller;

    void Start() {
        my_controller.on_death += OnDeath;
    }

    void OnDeath(ActorState form) {
        balor_lord.GetComponent<GenericActorController>().Hurt(9998);
        GameManagerScript.instance().MessageLog.NewMessage(
            "Balor lord has been gravely injured!", Color.red
        );
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
