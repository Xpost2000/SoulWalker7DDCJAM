using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum AnimationType {
    None,
    Movement,
    Rotation
};

public class PlayerController : MonoBehaviour
{
    public InputAction movement_action;
    public InputAction turn_view;
    public InputAction attack;

    /* TODO: interpolated/smooth motion */
    private Vector3 logical_position;
    private float   logical_rotation;

    /*
      I honestly have no idea when to consider it safe to take a turn programmatically right now,
      so I'm going to enforce that no animation timer is running before the player can make a move.

      NOTE: A turn is determined by a turn timer or when the player makes a move.
    */
    public bool animation_interpolate = true;
    private AnimationType anim_type = AnimationType.None;
    private float anim_timer = 0;

    // animation data
    private Vector3 start_lerp_position;
    private float   start_lerp_rotation_angle;

    void Start() {
        movement_action.started += OnMovementStart;
        turn_view.started += OnTurnStart;

        logical_position = transform.position;
        logical_rotation = transform.eulerAngles.y;
    }

    void OnEnable() {
        movement_action.Enable();
        turn_view.Enable();
        attack.Enable();
    }

    void StartAnimation(AnimationType type, float time) {
        if (!animation_interpolate) return;
        anim_type = type;
        anim_timer = time;
        start_lerp_position = new Vector3(logical_position.x, logical_position.y, logical_position.z);
        start_lerp_rotation_angle = logical_rotation;
    }

    void OnTurnStart(InputAction.CallbackContext ctx) {
        if (anim_type != AnimationType.None) return;

        StartAnimation(AnimationType.Rotation, 0.5f);

        var turn = (int)ctx.ReadValue<float>();
        logical_rotation += 90 * turn;
    }

    void OnMovementStart(InputAction.CallbackContext ctx) {
        if (anim_type != AnimationType.None) return;
        var movement = ctx.ReadValue<Vector2>();
        StartAnimation(AnimationType.Movement, 0.5f);

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

    void OnDisable() {
        movement_action.Disable();
        turn_view.Disable();
        attack.Disable();
    }

    void FixedUpdate() {
    }

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
                float effective_t = Mathf.Max(anim_timer, 0.0f);
                transform.position = Vector3.Lerp(logical_position, start_lerp_position, effective_t);

                anim_timer -= dt;
                if (anim_timer <= 0.0) {
                    anim_type = AnimationType.None;
                }
            } break;
            case AnimationType.Rotation: {
                float effective_t = Mathf.Max(anim_timer, 0.0f);
                transform.eulerAngles =
                    new Vector3(
                        transform.eulerAngles.x,
                        Mathf.Lerp(logical_rotation, start_lerp_rotation_angle, effective_t),
                        transform.eulerAngles.z
                    );
                
                anim_timer -= dt;
                if (anim_timer <= 0.0) {
                    anim_type = AnimationType.None;
                }
            } break;
        }
    }
}
