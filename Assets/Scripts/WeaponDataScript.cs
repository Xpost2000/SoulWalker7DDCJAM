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
    public GameObject projectile = null;

    // I don't know why I'm making this a float when I'm using
    // a grid but okay.
    public float distance = 1.0f;
    public MeleeAnimType melee_anim_type;

    // can modify the amount of damage we want to do.
    // or do other stuff
    public delegate void OnDamageFire(GameObject victim);
    public delegate bool OnProjectileDamageFire(GameObject original_projectile);
    public event OnProjectileDamageFire on_projectile_fire;
    public event OnDamageFire on_attack_damage;
    public event OnDamageFire on_attack_hit; // if you want to hit anything that isn't a controller object.

    // Start is called before the first frame update
    void Start() {
        
    }

    void Attack() {
        if (projectile != null) {
            // projectile path.
            // ignores 90% of this data lol
            GameManagerScript.instance().MessageLog.NewMessage(
                gameObject.name + " should be shooting stuff!", Color.white
            );
            bool? result = on_projectile_fire?.Invoke(projectile);
            if (result != null && result == true) {
                // allow firing the original projectile using the
                // normal script.
                GameManagerScript.instance().MessageLog.NewMessage(
                    "Overridden projectile behavior!", Color.yellow
                );
            } else {
                // disallow, the callback handled it.
            }
        } else {
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

            foreach (RaycastHit hit in hits) {
                var collider = hit.collider;
                var collider_gameObject = hit.collider.gameObject;

                if (collider_gameObject == holder) continue;
                else {
                    var controller_component = collider_gameObject.GetComponent<GenericActorController>();
                    if (controller_component) {
                        on_attack_damage?.Invoke(collider_gameObject);
                    }
                    on_attack_hit?.Invoke(collider_gameObject);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
