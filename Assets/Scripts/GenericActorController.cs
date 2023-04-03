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

    // TODO: add considerations for soul mode since all entities can go into spirit mode

    public int max_health = 20;
    public int health = 20;
    public int max_soul_health = -1;
    public int soul_health = -1;

    // this changes in soul mode.
    // depends on the player.
    public int defense = 5;

    /* animation data */
    private AnimationType anim_type = AnimationType.None;
    private float anim_timer = 0;
    private float max_lerp_time = 0;
    private Vector3 start_lerp_position;
    private float   start_lerp_rotation_angle;

    private static float ANIM_TIME_ACTOR = 0.3f;
    private static float ANIM_TIME_MOVEMENT = ANIM_TIME_ACTOR;
    private static float ANIM_TIME_ROTATE = ANIM_TIME_ACTOR;

    /*
      events
     */
    public delegate void OnHurt(int amount, ActorState damage_form);
    public delegate void OnDeath(ActorState death_form);
    public delegate void OnPickup(GameObject item);
    public event OnPickup on_pickup;
    public event OnHurt on_hurt;
    public event OnDeath on_death;
    /*
      end events
     */

    // Start is called before the first frame update
    void Start() {
        logical_position = transform.position;
        logical_rotation = transform.eulerAngles.y;
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
    bool ConsiderCollidingWith(string tagname) {
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
    public void MoveDirection(Vector2 direction) {
        if (anim_type != AnimationType.None) return;
        var movement = direction;
        StartAnimation(AnimationType.Movement, ANIM_TIME_MOVEMENT);

        RaycastHit raycast_result;
        bool hit_anything = false;

        bool is_on_ramp = false;

        // collision pickup logic is here lol!
        if (Physics.Raycast(logical_position, -transform.up, out raycast_result)) {
            is_on_ramp = (raycast_result.collider.gameObject.tag ==  "Ramp");
        }

        if (Physics.Raycast(logical_position, transform.forward * movement.y, out raycast_result, 1) ||
            Physics.Raycast(logical_position, transform.right * movement.x, out raycast_result, 1)) {
            hit_anything = ConsiderCollidingWith(raycast_result.collider.gameObject.tag);
        }

        if (!hit_anything) logical_position += transform.forward * movement.y + transform.right * movement.x; 

        // realign with floor
        print("Align with floor");
        if (Physics.Raycast(logical_position + transform.up, -transform.up, out raycast_result)) {
            if (ConsiderCollidingWith(raycast_result.collider.gameObject.tag)) {
                logical_position = new Vector3(logical_position.x, raycast_result.point.y+1, logical_position.z);
            }
        }

    }

    void OnTriggerEnter(Collider collider) {
        var collider_object = collider.gameObject;
        if (collider_object.tag == "Pickup") {
            if (gameObject.tag == "Player") {
                collider_object.GetComponent<ItemPickupGeneric>().InvokeOnTrigger(gameObject);
                on_pickup?.Invoke(collider_object.GetComponent<ItemPickupGeneric>().reward_item);
                print("Hi pickup!");
            } else {
            }
        } else {
        }
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
        int actual_damage = health-this.defense/2;
        this.soul_health -= (actual_damage);
        on_hurt?.Invoke(actual_damage, ActorState.Soul);
        if (this.soul_health <= 0) {
            on_death?.Invoke(ActorState.Soul);
        }
    }

    public void HurtBody(int health) {
        int actual_damage = health-this.defense;
        this.health -= (actual_damage);
        on_hurt?.Invoke(actual_damage, ActorState.Body);
        if (this.health <= 0) {
            on_death?.Invoke(ActorState.Body);
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

    public void Rotate(int direction) {
        if (anim_type != AnimationType.None) return;
        StartAnimation(AnimationType.Rotation, ANIM_TIME_ROTATE);
        var turn = direction;
        logical_rotation += 90 * turn;
    }

    // Update is called once per frame
    void Update() {
        float dt = Time.deltaTime;

        switch (anim_type) {
            case AnimationType.None: {
                transform.position = logical_position;
                transform.eulerAngles =
                    new Vector3(
                        transform.eulerAngles.x,
                        logical_rotation,
                        transform.eulerAngles.z
                    );
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
