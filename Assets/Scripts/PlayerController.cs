using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputAction movement_action;
    public InputAction turn_view;
    public InputAction attack;

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
        if (Physics.Raycast(transform.position, transform.forward * movement.y, out raycast_result, 1)) {
            print("Okay we hit something? (forward)");
            print(raycast_result);
            print(raycast_result.collider.gameObject.name);
            print(raycast_result.point);
            hit_anything = true;
        }
        if (Physics.Raycast(transform.position, transform.right * movement.x, out raycast_result, 1)) {
            print("Okay we hit something? (right)");
            print(raycast_result);
            print(raycast_result.collider.gameObject.name);
            print(raycast_result.collider);
            print(raycast_result.point);
            hit_anything = true;
        }

        if (!hit_anything) transform.position += transform.forward * movement.y + transform.right * movement.x; 
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
