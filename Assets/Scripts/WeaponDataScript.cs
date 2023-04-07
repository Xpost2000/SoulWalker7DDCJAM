using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This literally just holds data. Amazing.
public enum MeleeAnimType {
    Sword, // diagonal slash
    Spear  // thrust forward
};

public class WeaponDataScript : MonoBehaviour
{
    // If this is non-null this weapon is a ranged weapon.
    // Otherwise it is a raycast melee
    public GameObject holder = null; // set when equipped.
    // I don't know why I'm making this a float when I'm using
    // a grid but okay.
    public float distance = 1.0f;

    // Just do a raycast.
    public bool is_ranged = false;
    public MeleeAnimType melee_anim_type;

    // can modify the amount of damage we want to do.
    // or do other stuff
    public delegate void OnDamageFire(GameObject victim);
    public event OnDamageFire on_attack_damage;
    public event OnDamageFire on_attack_hit; // if you want to hit anything that isn't a controller object.

    // Start is called before the first frame update
    void Start() {
        
    }

    public void Attack() {
        // melee path
        if (holder == null) {
            GameManagerScript.instance().MessageLog.NewMessage(
                "HEY THIS IS A BUG! PLEASE TELL ME YOU DON'T SEE THIS AT ALL!", Color.red
            );
            return;
        }

        // NOTE: animation happens independent of raycast.
        // animation doesn't really mean anything since I don't have time to do
        // parrying like I wanted to.

        // raycast all
        RaycastHit[] hits =
            Physics.RaycastAll(holder.transform.position, holder.transform.forward, distance);

        print("Beginning raycasts");
        Vector3 start = holder.transform.position;
        Vector3 end = start;
        foreach (RaycastHit hit in hits) {
            print(hit);
            var collider = hit.collider;
            var collider_gameObject = hit.collider.gameObject;

            print(collider_gameObject);
            print(collider_gameObject.name);
            if (collider_gameObject == holder) {
                print("hit myself.");
                continue;
            }
            else {
                var controller_component = collider_gameObject.GetComponent<GenericActorController>();
                if (controller_component) {
                    print("Okay, hurtin! (WeaponDataScript)");
                    print(collider_gameObject);
                    on_attack_damage?.Invoke(collider_gameObject);
                    break;
                }
                on_attack_hit?.Invoke(collider_gameObject);

                end = hit.point;
            }
        }
    }
}
