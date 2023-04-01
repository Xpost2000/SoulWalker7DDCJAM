using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputAction movement_action;
    public InputAction turn_view;
    public InputAction attack;

    /* TODO: interpolated/smooth motion */
    private Vector3 logical_position;
    private float   logical_rotation;

    void Start() {
        movement_action.started += OnMovementStart;
        turn_view.started += OnTurnStart;
    }

    void OnEnable() {
        movement_action.Enable();
        turn_view.Enable();
        attack.Enable();
    }

    void OnTurnStart(InputAction.CallbackContext ctx) {
        var turn = (int)ctx.ReadValue<float>();
        transform.eulerAngles =
            new Vector3(
                transform.eulerAngles.x,
                transform.eulerAngles.y + 90 * turn,
                transform.eulerAngles.z
            );
    }

    void OnMovementStart(InputAction.CallbackContext ctx) {
        var movement = ctx.ReadValue<Vector2>();

        RaycastHit raycast_result;
        bool hit_anything = false;

        bool is_on_ramp = false;
        if (Physics.Raycast(transform.position, -transform.up, out raycast_result)) {
            is_on_ramp = (raycast_result.collider.gameObject.tag ==  "Ramp");
        }

        if (Physics.Raycast(transform.position, transform.forward * movement.y, out raycast_result, 1)) {
            hit_anything = true;
        }
        if (Physics.Raycast(transform.position, transform.right * movement.x, out raycast_result, 1)) {
            hit_anything = true;
        }

        if (!hit_anything) transform.position += transform.forward * movement.y + transform.right * movement.x; 

        // realign with floor
        print("Align with floor");

        // TODO: move this out to some other controller thingy
        // assume the alignment is always one object higher.
        if (Physics.Raycast(transform.position + transform.up, -transform.up, out raycast_result)) {
            transform.position = new Vector3(transform.position.x, raycast_result.point.y+1, transform.position.z);
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
    }
}
