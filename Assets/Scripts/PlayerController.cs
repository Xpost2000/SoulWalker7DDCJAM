using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputAction movement_action;
    public InputAction turn_view;
    public InputAction attack;

    // Don't know the best way to request this from
    // the input action without making so many separate
    // actions.
    private bool moved_horizontally_last = false;
    private bool moved_vertically_last = false;

    private bool turned_last = false;

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
        transform.position += transform.forward * movement.y + transform.right * movement.x; 
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
