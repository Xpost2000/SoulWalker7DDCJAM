using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnLocation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // this is just for the editor.
        GetComponent<MeshRenderer>().enabled = false;
        print("Hi, I'm going to place the player at me!");

        GameManagerScript.
            instance().player.
            GetComponent<GenericActorController>().SetLogicalPosition(transform.position);
        GameManagerScript
            .instance().player.
            GetComponent<GenericActorController>().SetLogicalRotation(transform.eulerAngles.y);

        // Now I die!
        print("Now I die!");
        Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
