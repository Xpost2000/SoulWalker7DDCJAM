using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using TMPro;

/*
  This isn't technically just a controller. A lot of the
  logic is also here. I know it's kinda dumb
 */
public class PlayerController : MonoBehaviour
{
    public InputAction movement_action;
    public InputAction turn_view;
    public InputAction attack;
    public InputAction pause_game;

    public GenericActorController controller;
    public PunchableCamera camera;
    public GameObject inventory_display_container;

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

    public bool InventoryActive() {
        return (inventory_display_container.activeSelf);
    }

    /*
      Basically I have a 3D UI so I instantiate replicas of the
      objects.

      Then I check their components to see what type they are and then
      I handle them specifically based on that...

      Yeah I don't really think it's a great idea either but this works I guess.
     */
    void OnItemPickup(GameObject what_item) {
        var healing_component = what_item.GetComponent<HealingItem>();
        if (healing_component) {
            GameManagerScript.instance().MessageLog.NewMessage("Picked up an ether!", Color.green);
        }

        /*
          Ah yeah this is called I should've looked up a tutorial before I did
          whatever the hell this is.
        */
        var replica = Instantiate(what_item, inventory_display_container.transform);
        replica.GetComponent<CapsuleCollider>().enabled = false;
        replica.GetComponent<ItemPickupGeneric>().enabled = true;

        if (healing_component) {
            replica.GetComponent<HealingItem>().enabled = true;
            // add a label I guess
            GameObject text = new GameObject("description");
            text.transform.SetParent(replica.transform);
            text.transform.localScale = new Vector3(1,1,1);
            TextMeshProUGUI text_data = text.AddComponent<TextMeshProUGUI>();
            text_data.fontSize = 20;
            text_data.text = "Hello World!";
        }

        replica.transform.localPosition = Vector3.zero;
        replica.layer = LayerMask.NameToLayer("UI");
        items.Add(replica);
        print("Added " + replica + " to the inventory.");
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

        // Should be a list of prefabs afaik
        items = new List<GameObject>();
        Close3DInventory();
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

    public void Close3DInventory() {
        inventory_display_container.SetActive(false);
    }

    public void Open3DInventory() {
        inventory_display_container.SetActive(true);
    }

    public void ToggleInventory() {
        if (InventoryActive()) {
            Close3DInventory();
        } else {
            Open3DInventory();
        }
    }

    void OnAttack(InputAction.CallbackContext ctx) {
        ToggleInventory();
        // controller.Hurt(10);
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
