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

public class GenericActorController : MonoBehaviour {
    private Vector3 logical_position;
    private float   logical_rotation;

    public int health = 20;
    public int defense = 5;

    /* animation data */
    public bool animation_interpolate = true;
    private AnimationType anim_type = AnimationType.None;
    private float anim_timer = 0;
    private float max_lerp_time = 0;
    private Vector3 start_lerp_position;
    private float   start_lerp_rotation_angle;

    private static float ANIM_TIME_ACTOR = 0.3f;
    private static float ANIM_TIME_MOVEMENT = ANIM_TIME_ACTOR;
    private static float ANIM_TIME_ROTATE = ANIM_TIME_ACTOR;

    // Start is called before the first frame update
    void Start() {
        logical_position = transform.position;
        logical_rotation = transform.eulerAngles.y;
    }

    void StartAnimation(AnimationType type, float time) {
        if (!animation_interpolate) return;
        anim_type = type;
        max_lerp_time = time;
        anim_timer = 0;
        start_lerp_position = new Vector3(logical_position.x, logical_position.y, logical_position.z);
        start_lerp_rotation_angle = logical_rotation;
    }

    // NOTE: no one can hover, since we always snap to the floor
    // TODO: provide events on specific item collisions.
    // or bit flags on behaviors
    public void MoveDirection(Vector2 direction) {
        if (anim_type != AnimationType.None) return;
        var movement = direction;
        StartAnimation(AnimationType.Movement, ANIM_TIME_MOVEMENT);

        RaycastHit raycast_result;
        bool hit_anything = false;

        bool is_on_ramp = false;

        if (Physics.Raycast(logical_position, -transform.up, out raycast_result)) {
            is_on_ramp = (raycast_result.collider.gameObject.tag ==  "Ramp");
        }

        if (Physics.Raycast(logical_position, transform.forward * movement.y, out raycast_result, 1)) {
            hit_anything = true;
        }
        if (Physics.Raycast(logical_position, transform.right * movement.x, out raycast_result, 1)) {
            hit_anything = true;
        }

        if (!hit_anything) logical_position += transform.forward * movement.y + transform.right * movement.x; 

        // realign with floor
        print("Align with floor");
        if (Physics.Raycast(logical_position + transform.up, -transform.up, out raycast_result)) {
            logical_position = new Vector3(logical_position.x, raycast_result.point.y+1, logical_position.z);
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
