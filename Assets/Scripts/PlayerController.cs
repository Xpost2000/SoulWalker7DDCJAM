using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputAction movement_action;
    public InputAction turn_view;
    public InputAction attack;
    public InputAction pause_game;

    public GenericActorController controller;
    public PunchableCamera camera;

    int selected_item_index = 0;
    public List<GameObject> items;

    public void DisableInput() {
        movement_action.Disable();
        turn_view.Disable();
        attack.Disable();
    }
    public void EnableInput() {
        movement_action.Enable();
        turn_view.Enable();
        attack.Enable();
        // NOTE: this is never disabled
        pause_game.Enable();
    }

    void OnItemPickup(GameObject what_item) {
        var healing_component = what_item.GetComponent<HealingItem>();
        if (healing_component) {
            GameManagerScript.instance().MessageLog.NewMessage("Picked up an ether!", Color.green);
        }

        items.Add(what_item);
    }

    void Start() {
        movement_action.started += OnMovementStart;
        turn_view.started += OnTurnStart;
        attack.started += OnAttack;
        pause_game.started += OnPauseGame;

        controller = GetComponent<GenericActorController>();
        camera = GetComponent<PunchableCamera>();

        controller.on_hurt += OnHurt;
        controller.on_death += OnDeath;
        controller.on_pickup += OnItemPickup;

        items = new List<GameObject>();
    }

    void OnDeath() {
        GameManagerScript.instance().State = GameState.GameOver;
        pause_game.Disable();
    }

    void OnHurt(int health) {
        // move the camera
        camera.Traumatize(0.12f);
        GameManagerScript.instance().MessageLog.NewMessage("Player has been hurt for " + health.ToString() + " damage!", Color.red);
    }

    void OnPauseGame(InputAction.CallbackContext ctx) {
        if (GameManagerScript.instance().State != GameState.Pause) {
            GameManagerScript.instance().State = GameState.Pause;
        } else {
            GameManagerScript.instance().State = GameState.Ingame;
        }
    }

    void OnAttack(InputAction.CallbackContext ctx) {
        controller.Hurt(10);
        // GameManagerScript.instance().MessageLog.NewMessage("Player attacks!", Color.white);
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
