using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
  A generic actor is just anything in this game that can move/think.

  It just supplies rotation/movement and death behaviors,
  attach to any game object with a collider or something.

  NOTE: if I want to do attacks, I need to do something else special...

  TODO: handle turn start actions? (to check what's on the floor for instance?)
*/
public enum AnimationType {
    None,
    Movement,
    Rotation
};

[System.Flags] // NOTE for trigger types that are deadly!
public enum ActorState {
    None = 0,
    Soul = 1,
    Body = 2,
};

public class GenericActorController : MonoBehaviour {
    private Vector3 logical_position;
    private float   logical_rotation;
    public ActorState form;
    public AudioSource soundsource;

    public int max_health = 25;
    public int health = 25;
    public int max_soul_health = -1;
    public int soul_health = -1;

    // this changes in soul mode.
    // depends on the player.
    public int defense = 2;

    /* animation data */
    private AnimationType anim_type = AnimationType.None;
    private float anim_timer = 0;
    private float max_lerp_time = 0;
    private Vector3 start_lerp_position;
    private float   start_lerp_rotation_angle;

    private static float ANIM_TIME_ACTOR = 0.15f;
    private static float ANIM_TIME_MOVEMENT = ANIM_TIME_ACTOR;
    private static float ANIM_TIME_ROTATE = ANIM_TIME_ACTOR;

    /*
      events
     */
    public delegate void OnHurt(int amount, ActorState damage_form);
    public delegate void OnDeath(ActorState death_form);
    public event OnHurt on_hurt;
    public event OnDeath on_death;
    /*
      end events
     */

    public void SetLogicalPosition(Vector3 position) {
        logical_position = position;
    }

    public void SetLogicalRotation(float angle) {
        logical_rotation = angle;
    }

    void Start() {
        SetLogicalPosition(transform.position);
        SetLogicalRotation(transform.eulerAngles.y);
        max_health = health;

        if (max_soul_health == -1 ||
            soul_health == -1) {
            print("NOTE: no override on soul health!");
            // Soul health is always half the actual health of a character
            // if it is not explicitly initialized
            max_soul_health = soul_health = (max_health/2);
        }
    }

    void StartAnimation(AnimationType type, float time) {
        if (!GameManagerScript.instance().interpolate_animations) return;
        anim_type = type;
        max_lerp_time = time;
        anim_timer = 0;
        start_lerp_position = new Vector3(logical_position.x, logical_position.y, logical_position.z);
        start_lerp_rotation_angle = logical_rotation;
    }

    // NOTE: no one can hover, since we always snap to the floor
    // TODO: provide events on specific item collisions.
    // or bit flags on behaviors
    bool ConsiderCollidingWith(GameObject obj, string tagname) {
        if (obj == gameObject) return false;
        switch (tagname) {
            case "Pickup":
            case "Nonsolid": {
                return false;
            } break;
            case "SolidForSoulOnly": {
                if (form == ActorState.Soul) return true;
                else return false;
            } break;
            case "SolidForBodyOnly": {
                if (form == ActorState.Soul) return false;
                else return true;
            } break;
        }
        return true;
    }

    // void RealignWithFloor

    public bool MoveDirection(Vector2 direction) {
        if (anim_type != AnimationType.None) return false;
        var movement = direction;
        StartAnimation(AnimationType.Movement, ANIM_TIME_MOVEMENT);

        if (soundsource != null) {
            soundsource.clip = GameManagerScript.instance().sound_step;
            soundsource.Play();
        }

        RaycastHit raycast_result;
        bool hit_anything = false;

        int layer_mask = 1 << LayerMask.NameToLayer("Default");

        if (Physics.Raycast(logical_position, transform.forward * movement.y, out raycast_result, 1, layer_mask, QueryTriggerInteraction.Ignore) ||
            Physics.Raycast(logical_position, transform.right * movement.x, out raycast_result, 1, layer_mask, QueryTriggerInteraction.Ignore)) {
            hit_anything = ConsiderCollidingWith(raycast_result.collider.gameObject, raycast_result.collider.gameObject.tag);
        }

        if (!hit_anything) logical_position += transform.forward * movement.y + transform.right * movement.x; 

        // realign with floor
        print("Align with floor");
        if (Physics.Raycast(logical_position + transform.up, -transform.up, out raycast_result, float.PositiveInfinity, layer_mask, QueryTriggerInteraction.Ignore)) {
            print(raycast_result.collider.gameObject);
            if (ConsiderCollidingWith(raycast_result.collider.gameObject, raycast_result.collider.gameObject.tag)) {
                logical_position = new Vector3(logical_position.x, raycast_result.point.y+1, logical_position.z);
            }
        }

        return !hit_anything;
    }

    public bool MoveAbsoluteDirection(Vector2 xy) {
        // only accept x or y. Not X and Y. Sorry!
        // x first. y second.
        if((int)xy.x == 0 && (int)xy.y == 0) return false;
        
        if ((int)xy.x != 0) {
            GameManagerScript.instance().MessageLog.NewMessage(gameObject.name + " thinks to move X mode!", Color.red);
            if (xy.x < 0) {
                SetLogicalRotation(-90.0f);
            } else if (xy.x > 0) {
                SetLogicalRotation(90.0f);
            }
        } else {
            GameManagerScript.instance().MessageLog.NewMessage(gameObject.name + " thinks to move Y mode!", Color.red);
            if (xy.y < 0) {
                SetLogicalRotation(180.0f);
            } else if (xy.y > 0) {
                SetLogicalRotation(0.0f);
            }
        }
        SyncLocalToActual();
        Debug.DrawRay(transform.position, transform.forward * 2, Color.green, 5);
        // always move forward after turning.
        return MoveDirection(Vector2.up);
    }

    public void HealSoul(int health) {
        this.soul_health += health;
        if (this.soul_health > this.max_soul_health) {
            this.soul_health = this.max_soul_health;
        }
    }
    public void HealBody(int health) {
        this.health += health;
        if (this.health > this.max_health) {
            this.health = this.max_health;
        }
    }

    public bool EquipBody(GameObject body_object) {
        if (form != ActorState.Body) {
            var body = body_object.GetComponent<BodyPickupScript>();
            health = body.health;
            max_health = body.max_health;

            Destroy(body_object);
            form = ActorState.Body;
            return true;
        }
        return false;
    }

    public bool UnequipBody() {
        // count as a body death
        if (form != ActorState.Soul) {
            var replica = Instantiate(GameManagerScript.instance().body_pickup_prefab);
            replica.GetComponent<BodyPickupScript>().health = health;
            replica.GetComponent<BodyPickupScript>().max_health = max_health;

            RaycastHit raycast_result;

            // Create a body where we currently are.
            replica.transform.position = transform.position;
            if (Physics.Raycast(logical_position, -transform.up, out raycast_result)) {
                replica.transform.position =  new Vector3(
                    transform.position.x,
                    raycast_result.point.y,
                    transform.position.z
                );
            }

            health = max_health = 0;
            form = ActorState.Soul;
            return true;
        }

        return false;
    }

    // generic heal
    public void Heal(int health) {
        switch (form) {
            case ActorState.Body: {
                HealBody(health);
            } break;
            case ActorState.Soul: {
                HealSoul(health);
            } break;
        }
    }

    public float Health {
        get {
            switch (form) {
                case ActorState.Body: {
                    return health;
                } break;
                case ActorState.Soul: {
                    return soul_health;
                } break;
            }
            return 0.0f;
        }
    }
    public float MaxHealth {
        get {
            switch (form) {
                case ActorState.Body: {
                    return max_health;
                } break;
                case ActorState.Soul: {
                    return max_soul_health;
                } break;
            }
            return 0.0f;
        }
    }

    public float HealthPercent {
        get {
            return Health / MaxHealth;
        }
    }

    public void HurtSoul(int health) {
        if (health > 0) {
            print("Hurt soul!");
            if (this.soul_health > 0) {
                int actual_damage = health-this.defense/2;
                this.soul_health -= (actual_damage);
                on_hurt?.Invoke(actual_damage, ActorState.Soul);
                if (this.soul_health <= 0) {
                    on_death?.Invoke(ActorState.Soul);
                }
            }
        } else if (health < 0) {
            HealSoul(-health);
        }
    }

    public void HurtBody(int health) {
        if (health > 0) {
            if (this.health > 0) {
                print("Hurt body!");
                int actual_damage = health-this.defense;

                // min damage is 1, 0 damage should be impossible.
                if (actual_damage <= 0) actual_damage = 1;

                this.health -= (actual_damage);
                on_hurt?.Invoke(actual_damage, ActorState.Body);
                if (this.health <= 0) {
                    on_death?.Invoke(ActorState.Body);
                }
            }
        } else if (health < 0) {
            HealBody(-health);
        }
    }

    // generic class hurt
    public void Hurt(int health) {
        switch (form) {
            case ActorState.Body: {
                HurtBody(health);
            } break;
            case ActorState.Soul: {
                HurtSoul(health);
            } break;
        }
    }

    public bool Rotate(int direction) {
        if (anim_type != AnimationType.None) return false;
        StartAnimation(AnimationType.Rotation, ANIM_TIME_ROTATE);
        var turn = direction;
        logical_rotation += 90 * turn;
        return true;
    }

    // Update is called once per frame
    void SyncLocalToActual() {
        transform.position = logical_position;
        transform.eulerAngles =
            new Vector3(
                transform.eulerAngles.x,
                logical_rotation,
                transform.eulerAngles.z
            );
    }
    void Update() {
        float dt = Time.deltaTime;

        switch (anim_type) {
            case AnimationType.None: {
                SyncLocalToActual();
            } break;
            case AnimationType.Movement: {
                float effective_t = Mathf.Clamp(anim_timer/max_lerp_time, 0.0f, 1.0f);
                transform.position = Vector3.Lerp(start_lerp_position, logical_position, effective_t);
            } break;
            case AnimationType.Rotation: {
                float effective_t = Mathf.Clamp(anim_timer/max_lerp_time, 0.0f, 1.0f);
                transform.eulerAngles =
                    new Vector3(
                        transform.eulerAngles.x,
                        Mathf.Lerp(start_lerp_rotation_angle, logical_rotation, effective_t),
                        transform.eulerAngles.z
                    );
                
            } break;
        }

        if (anim_timer >= max_lerp_time) {
            anim_type = AnimationType.None;
        }
        
        anim_timer += dt;
    }
}
