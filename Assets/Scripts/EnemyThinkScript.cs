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

  Expects:
  a collider to determine "FOV",
  and the two other controller types which submanage other parts of
  the actor type.
*/
public class EnemyThinkScript : MonoBehaviour
{
    public float soul_damage_modifier = 1.0f;
    public float physical_damage_modifier = 1.0f;
    public int damage = 5;
    public bool can_do_ranged = false;
    public float attack_range = 3.0f; // NOTE can be less than chase collider
    // +/- range
    public int damage_variance = 2;
    public bool allow_ranged_attacks = false;
    public float attack_hitting_chance = 0.67f;

    /* chase data cause it's funny */
    public bool can_wander = false;
    public float wander_radius = 4;
    public Vector3 starting_position;


    GameObject player;
    // Start is called before the first frame update

    GenericActorController controller;
    EnemyActorController   enemy_controller;

    void Start() {
        enemy_controller = GetComponent<EnemyActorController>();
        controller       = GetComponent<GenericActorController>();

        enemy_controller.on_turn_start += OnTurnStart;
        enemy_controller.on_turn_end += OnTurnEnd;
        starting_position =  transform.position;
    }

    void OnDestroy() {}

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

    void HandleWanderBehavior() {
        print("Okay. Let's do a wandering behavior!");
        // TODO;
    }

    void HandleChasingBehavior() {
        print("Okay. Let's do a chasing behavior!");
        // TODO;
    }

    void HandleAttackingBehavior() {
        print("Let's see if I can attack.");
        if (can_do_ranged) {
        }
    }

    void OnTurnEnd() {
        bool is_chasing = player != null;
        if (is_chasing) {
            HandleChasingBehavior();
        } else {
            if (can_wander) {
                HandleWanderBehavior();
            }
        }

        HandleAttackingBehavior();
    }

    void OnTurnStart() {
    }
}
