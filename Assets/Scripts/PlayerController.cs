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
    public InputAction confirm_use;
    public InputAction pause_game;

    public GenericActorController controller;
    public PunchableCamera camera;

    // LOL this should be elsewhere but okay.
    public GameObject inventory_display_container;
    public Canvas inventory_canvas_template_for_item;

    int selected_item_index = 0;
    public List<GameObject> items;

    public void DisableInput() {
        movement_action.Disable();
        turn_view.Disable();
        attack.Disable();
        confirm_use.Disable();
    }
    public void EnableInput() {
        movement_action.Enable();
        turn_view.Enable();
        attack.Enable();
        confirm_use.Enable();
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

            var replica_canvas =
                Instantiate(
                    inventory_canvas_template_for_item,
                    replica.transform
                );
            replica_canvas.transform.localPosition = Vector3.zero;

            // add a label I guess
            GameObject text = new GameObject("description");
            text.transform.SetParent(replica_canvas.transform);
            text.transform.localScale = new Vector3(1,1,1);
            text.transform.localPosition = new Vector3(0, -2f, 0);
            TextMeshProUGUI text_data = text.AddComponent<TextMeshProUGUI>();
            text_data.fontSize = 32;
            text_data.text = healing_component.description;
        }

        replica.transform.localPosition = new Vector3(0.0f, -0.2f, 0.0f);
        replica.layer = LayerMask.NameToLayer("UI");
        items.Add(replica);
        print("Added " + replica + " to the inventory.");
    }

    void Start() {
        movement_action.started += OnMovementStart;
        turn_view.started += OnTurnStart;
        attack.started += OnAttack;
        pause_game.started += OnPauseGame;
        confirm_use.started += OnConfirmOrUse;

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

    void OnConfirmOrUse(InputAction.CallbackContext ctx) {
        if (InventoryActive()) {
            UseInventoryItem(selected_item_index);
        } else {
            // fire a raycast and see if we hit a door?
            // if a door is locked we'll check if we have the key item I suppose.
        }
    }

    public void Close3DInventory() {
        inventory_display_container.SetActive(false);
    }

    void ValidateItemIndexBounds() {
        if (selected_item_index <= 0)
            selected_item_index = 0;
        else if (selected_item_index >= inventory_display_container.transform.childCount)
            selected_item_index = inventory_display_container.transform.childCount-1;
    }

    // sets it based off the index
    void Adjust3DInventoryChildrenLocation() {
        int i = 0;
        // space out each child.
        foreach (Transform child in inventory_display_container.transform) {
            child.localPosition = new Vector3(
                ((i) - selected_item_index)*0.7f,
                child.localPosition.y,
                child.localPosition.z);
            i++;
        }
    }

    public void Open3DInventory() {
        if (inventory_display_container.transform.childCount == 0) {
            GameManagerScript.instance().MessageLog.NewMessage("Inventory empty!", Color.red);
        } else {
            inventory_display_container.SetActive(true);
            // position all items
            Adjust3DInventoryChildrenLocation();
        }
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
        controller.Hurt(10);
        // GameManagerScript.instance().MessageLog.NewMessage("Player attacks!", Color.white);
    }

    void OnTurnStart(InputAction.CallbackContext ctx) {
        if (!InventoryActive()) {
            controller.Rotate((int)ctx.ReadValue<float>());
        }
    }

    void OnMovementStart(InputAction.CallbackContext ctx) {
        if (!InventoryActive()) {
            var movement = ctx.ReadValue<Vector2>();
            controller.MoveDirection(movement);
        } else {
            var movement = ctx.ReadValue<Vector2>();
            if (movement.x >= 0.0f) {
                selected_item_index += 1;
            } else if (movement.x <= 0.0f) {
                selected_item_index -= 1;
            }
            ValidateItemIndexBounds();
            Adjust3DInventoryChildrenLocation();
        }
    }

    void OnEnable() {EnableInput();}
    void OnDisable() {DisableInput();}

    // bad code or something
    public void UseInventoryItem(int index) {
        int i = 0;
        foreach (Transform child in inventory_display_container.transform) {
            if (i == index) {
                bool used = false;

                var healing_component = child.gameObject.GetComponent<HealingItem>();
                if (healing_component) {
                    GameManagerScript.instance().MessageLog.NewMessage("Healed " + healing_component.amount.ToString() + " hp!", Color.green);
                    controller.Heal(healing_component.amount);
                    used = true;
                }

                if (used) {
                    child.parent = null;
                    Destroy(child.gameObject);
                    Close3DInventory();
                } else {
                    GameManagerScript.instance().MessageLog.NewMessage("Cannot use this item!", Color.red);
                }
                break;
            }
            i++;
        }
    }
}
