using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputAction movement_action;
    public InputAction turn_view;
    public InputAction attack;

    public GenericActorController controller;

    void Start() {
        movement_action.started += OnMovementStart;
        turn_view.started += OnTurnStart;
        controller = GetComponent<GenericActorController>();
    }

    void OnEnable() {
        movement_action.Enable();
        turn_view.Enable();
        attack.Enable();
    }

    void OnTurnStart(InputAction.CallbackContext ctx) {
        controller.Rotate((int)ctx.ReadValue<float>());
    }

    void OnMovementStart(InputAction.CallbackContext ctx) {
        var movement = ctx.ReadValue<Vector2>();
        controller.MoveDirection(movement);
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
