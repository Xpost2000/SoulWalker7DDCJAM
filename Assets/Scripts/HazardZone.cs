using System.Collections;
using System.Collections.Generic;

using System.Linq;
using UnityEngine;

public class HazardZone : MonoBehaviour
{
    public int damage = 0;
    public int soul_damage = -1;
    public ActorState dangerous_to = ActorState.None;

    // NOTE currently unused.
    public string[] blacklist_type_tags; // useful to prevent killing certain enemies

    // Intended to be used on triggers!

    private List<GameObject> victims;

    void OnTriggerEnter(Collider collider) {
        var collider_gameObject = collider.gameObject;

        if (collider_gameObject.GetComponent<GenericActorController>()) {
            victims.Add(collider_gameObject);
        }
    }

    void OnTriggerExit(Collider collider) {
        var collider_gameObject = collider.gameObject;

        if (collider_gameObject.GetComponent<GenericActorController>()) {
            victims.Remove(collider_gameObject);
        }
    }


    void Start() {
        victims = new List<GameObject>();
        GameManagerScript.instance().on_turn_start += OnTurnStart;
        GameManagerScript.instance().on_turn_end   += OnTurnEnd;

        if (soul_damage == -1) {
            soul_damage = damage * 2; // yeah these will hurt.
        }
    }

    void OnDestroy() {
        print("Bye! I'm dead!");
        GameManagerScript.instance().on_turn_start -= OnTurnStart;
        GameManagerScript.instance().on_turn_end   -= OnTurnEnd;
    }

    void OnTurnStart() {
        foreach (GameObject victim in victims) {
            var actor_component = victim.GetComponent<GenericActorController>();
            bool should_hurt = false;
            if ((actor_component.form & dangerous_to) != 0) {
                if (!blacklist_type_tags.Contains(victim.tag)) {
                    should_hurt = true;
                }
            }

            if (should_hurt) {
                if (actor_component.form == ActorState.Soul) {
                    actor_component.Hurt(soul_damage);
                } else {
                    actor_component.Hurt(damage);
                }
            }
        }
    }

    void OnTurnEnd() {
        // not sure.
    }
}
