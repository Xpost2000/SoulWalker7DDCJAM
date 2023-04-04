using System.Collections;
using System.Collections.Generic;

using UnityEngine;

/*
  Generic pickup component that other
  pickup types inherit from.

  This can be configured to give an item
  to the inventory I guess.
*/

public class ItemPickupGeneric : MonoBehaviour
{
    public delegate void OnPickup(GameObject actorToAward, GameObject item);
    public GameObject reward_item;
    public string description="itemname";
    public event OnPickup on_pickup;

    private float start_y;

    void Start() {
        // TODO change mesh
        start_y = transform.position.y;
    }

    public void InvokeOnTrigger(GameObject actorToAward) {
        print("Hi, I'm dying!");
        // unused
        on_pickup?.Invoke(actorToAward, Instantiate(reward_item));
        Destroy(this.gameObject);
    }

    private float normalized_sin(float t) {
        return ((Mathf.Sin(t)+1)/2.0f);
    }

    void Update() {
        float dt = Time.deltaTime;

        transform.position =
            new Vector3(
                transform.position.x,
                start_y + normalized_sin(Time.time)  * 0.3f,
                transform.position.z
            );

        transform.eulerAngles = new Vector3(
            transform.eulerAngles.x,
            transform.eulerAngles.y + dt * 20.0f,
            transform.eulerAngles.z
        );
    }
}
