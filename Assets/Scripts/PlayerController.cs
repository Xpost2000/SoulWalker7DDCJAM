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
    public PunchableCamera camera;

    public void DisableInput() {
        movement_action.Disable();
        turn_view.Disable();
        attack.Disable();
    }
    public void EnableInput() {
        movement_action.Enable();
        turn_view.Enable();
        attack.Enable();
    }

    void Start() {
        movement_action.started += OnMovementStart;
        turn_view.started += OnTurnStart;
        attack.started += OnAttack;

        controller = GetComponent<GenericActorController>();
        camera = GetComponent<PunchableCamera>();
        controller.on_hurt += OnHurt;
    }

    void OnHurt(int health) {
        // move the camera
        camera.Traumatize(0.12f);
    }

    void OnAttack(InputAction.CallbackContext ctx) {
        controller.Hurt(10);
    }

    void OnTurnStart(InputAction.CallbackContext ctx) {
        controller.Rotate((int)ctx.ReadValue<float>());
    }

    void OnMovementStart(InputAction.CallbackContext ctx) {
        var movement = ctx.ReadValue<Vector2>();
        controller.MoveDirection(movement);
    }

    void OnEnable() {EnableInput();}
    void OnDisable() {DisableInput();}

    void FixedUpdate() {
    }

    void Update() {
        float dt = Time.deltaTime;
    }
}
