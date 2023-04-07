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
    public float attack_hitting_chance = 0.67f;

    /* chase data cause it's funny */
    public bool can_wander = false;

    public float wander_radius = 4;
    public Vector3 starting_position;

    // higher is "slower"
    public int turn_between_moves = 1;
    int turn_timer = 0;


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

    public int GetDamage(ActorState state) {
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
        float wander_distance = Vector3.Distance(starting_position, transform.position);
        if (wander_distance < wander_radius) {
            var movements = new Vector2[] {Vector2.up, -Vector2.up, Vector2.right, -Vector2.right};
            controller.MoveAbsoluteDirection(
                movements[(int)Random.Range(0, 3)]
            );
        } else {
            // walk back
            var wander_back_direction =  starting_position - transform.position;
            controller.MoveAbsoluteDirection(new Vector2(wander_back_direction.x, wander_back_direction.z));
        }
    }

    void HandleChasingBehavior() {
        if (Vector3.Distance(player.transform.position, transform.position) >= 1.0f)  {
            print("Okay. Let's do a chasing behavior!");
            Vector3 travel_direction = player.transform.position - transform.position;
            Debug.DrawRay(transform.position, travel_direction * 2, Color.red, 5);
            controller.MoveAbsoluteDirection(new Vector2(travel_direction.x, travel_direction.z));
        } else {
            print("Close enough no need to chase.");
        }
    }

    void HandleAttackingBehavior() {
        print("Let's see if I can attack.");
        if (can_do_ranged && Vector3.Distance(player.transform.position, transform.position) < attack_range) {
            print("I can do aranged attack?");
            // do raycast
            RaycastHit[] hits =
                Physics.RaycastAll(transform.position, transform.forward, attack_range);

            print("Beginning raycasts " + hits.Length);
            Vector3 start = transform.position;
            Vector3 end = start;
            foreach (RaycastHit hit in hits) {
                print(hit);
                var collider = hit.collider;
                var collider_gameObject = hit.collider.gameObject;

                print(collider_gameObject);
                print(collider_gameObject.name);
                if (collider_gameObject == gameObject) {
                    print("hit myself.");
                    continue;
                }
                else if (collider_gameObject.tag == "Player") {
                    if (Random.Range(0.0f, 1.0f) < attack_hitting_chance) {
                        var controller_component = collider_gameObject.GetComponent<GenericActorController>();
                        if (controller_component) {
                            print("Okay, hurtin!");
                            controller_component.Hurt(GetDamage(controller_component.form));
                        }

                        end = hit.point;
                        break;
                    } else {
                        GameManagerScript.instance().MessageLog.NewMessage(gameObject.name + " misses!", Color.red);
                        return;
                    }
                }

            }

            // tracer summon
            GameObject replica = Instantiate(GameManagerScript.instance().prefab_projectile_tracer);
            replica.GetComponent<LineTracerScript>().SetPoints(start, start + transform.forward * attack_range);
        } else {
            print("I can do a melee attack?");
            // do melee check
            RaycastHit[] hits =
                Physics.RaycastAll(transform.position, transform.forward, attack_range);

            print("Beginning raycasts " + hits.Length);
            Vector3 start = transform.position;
            Vector3 end = start;
            foreach (RaycastHit hit in hits) {
                print(hit);
                var collider = hit.collider;
                var collider_gameObject = hit.collider.gameObject;

                print(collider_gameObject);
                print(collider_gameObject.name);
                if (collider_gameObject == gameObject) {
                    print("hit myself.");
                    continue;
                }
                else if (collider_gameObject.tag == "Player") {
                    if (Random.Range(0.0f, 1.0f) < attack_hitting_chance) {
                        var controller_component = collider_gameObject.GetComponent<GenericActorController>();
                        if (controller_component) {
                            print("Okay, hurtin!");
                            controller_component.Hurt(GetDamage(controller_component.form));
                        }

                        end = hit.point;
                        break;
                    } else {
                        GameManagerScript.instance().MessageLog.NewMessage(gameObject.name + " misses!", Color.red);
                        return;
                    }
                }

            }
        }
    }

    void OnTurnEnd() {
        bool is_chasing = player != null;
        if (turn_timer <= 0) {
            if (is_chasing) {
                HandleChasingBehavior();
            } else {
                if (can_wander) {
                    HandleWanderBehavior();
                }
            }

            if (player != null) HandleAttackingBehavior();
            turn_timer = turn_between_moves;
        } else {
            turn_timer -= 1;
        }
    }

    void OnTurnStart() {
    }
}
