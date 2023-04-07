using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
  last "minute" enemy character controller

  will chase player in a dumb way and will attack
  on turn if it is in range

  I can't really think of an elaborate way to get the AI
  working very cleverly so yeah.

  It's a some select behaviors I can toggle on or off.

  NOTE: meant to be used in conjunction with an enemy actor controller.

  In theory this would be nice to make a type of scriptable object since it's
  a "data" container.
*/
public class EnemyThinkScript : MonoBehaviour
{
    public float soul_damage_modifier = 1.0f;
    public float physical_damage_modifier = 1.0f;
    public int damage = 5;
    // +/- range
    public int damage_variance = 2;
    public bool allow_ranged_attacks = false;
    public float attack_hitting_chance = 0.67f;

    /**/
    public bool can_wander = false;
    public float wander_radius = 4;

    GameObject player;
    // Start is called before the first frame update
    void Start() {
        var enemy_actor_controller = GetComponent<EnemyActorController>();
        enemy_actor_controller.on_turn_start += OnTurnStart;
        enemy_actor_controller.on_turn_end += OnTurnEnd;
    }

    void OnDestroy() {
    }

    public int getDamage(ActorState state) {
        float modifier = 1.0f;
        if (state == ActorState.Body) modifier = physical_damage_modifier;
        else                          modifier = soul_damage_modifier;

        int random_variance = (int)Random.Range(-damage_variance, damage_variance);
        int result =  (int)(damage * modifier + random_variance);
        if (result <= 0) result = 0;
        return result;
    }

    void OnTriggerEnter(Collider collider) {
        var collider_gameObject = collider.gameObject;

        if (collider_gameObject.tag == "Player") {
            player = collider_gameObject;
            print("Player entered. Start chasing!");
        }
    }

    void OnTriggerExit(Collider collider) {
        var collider_gameObject = collider.gameObject;

        if (collider_gameObject.tag == "Player") {
            player = null;
            print("Player left. Stop chasing?");
        }
    }

    // Update is called once per frame
    void Update() {}

    void OnTurnEnd() {
    }

    void OnTurnStart() {
    }
}
