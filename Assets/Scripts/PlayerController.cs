using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using TMPro;

/*
  This isn't technically just a controller. A lot of the
  logic is also here. I know it's kinda dumb
 */
struct ActivePrompt {
    public enum Type{
        None,
        Body,
        Activatable, // doors or keys
    };
    public Type type;
    public GameObject subject;
};

public static class GameObjectExtension {
    public static void SetLayerRecursively(this GameObject obj, LayerMask mask) {
        obj.layer = mask;
        foreach (Transform child in obj.transform) {
            child.gameObject.SetLayerRecursively(mask);
        }
    }
}

public class PlayerController : MonoBehaviour
{
    public InputAction movement_action;
    public InputAction turn_view;
    public InputAction attack;
    public InputAction confirm_use;
    public InputAction open_inventory;
    public InputAction pause_game;
    public InputAction drop_body;

    public GenericActorController controller;
    public PunchableCamera camera;

    // LOL this should be elsewhere but okay.
    public GameObject inventory_display_container;
    public Canvas inventory_canvas_template_for_item;
    public List<GameObject> items;

    public float hit_chance = 0.8895f;

    private int selected_item_index = 0;

    private ActivePrompt prompt;

    GameObject active_weapon;

    public Animator animator;
    public GameObject viewlight;

    public void DisableInput() {
        movement_action.Disable();
        turn_view.Disable();
        attack.Disable();
        confirm_use.Disable();
        drop_body.Disable();
        open_inventory.Enable();
    }
    public void EnableInput() {
        movement_action.Enable();
        turn_view.Enable();
        attack.Enable();
        confirm_use.Enable();
        drop_body.Enable();
        open_inventory.Enable();
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
        var pickup_component = what_item.GetComponent<ItemPickupGeneric>();
        var healing_component = what_item.GetComponent<HealingItem>();
        if (healing_component) {
            GameManagerScript.instance().MessageLog.NewMessage("Picked up an ether!", Color.green);
        } else {
            GameManagerScript.instance().MessageLog.NewMessage("Picked up an item!", Color.green);
        }

        /*
          Ah yeah this is called I should've looked up a tutorial before I did
          whatever the hell this is.
        */
        var replica = Instantiate(what_item, inventory_display_container.transform);
        
        if (replica.GetComponent<CapsuleCollider>())
            replica.GetComponent<CapsuleCollider>().enabled = false;

        replica.GetComponent<ItemPickupGeneric>().enabled = true;

        if (healing_component) {
            replica.GetComponent<HealingItem>().enabled = true;
        }

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
        text.transform.localPosition = new Vector3(0, -2.5f, 0);
        TextMeshProUGUI text_data = text.AddComponent<TextMeshProUGUI>();
        text_data.fontSize = 32;
        text_data.text = pickup_component.description;

        replica.transform.localPosition = new Vector3(0.0f, -0.2f, 0.0f);
        replica.SetLayerRecursively(LayerMask.NameToLayer("UI"));
        items.Add(replica);
        print("Added " + replica + " to the inventory.");
    }

    void OnTriggerEnter(Collider collider) {
        var collider_object = collider.gameObject;
        if (collider_object.tag == "Pickup") {
            var pickup_component = collider_object.GetComponent<ItemPickupGeneric>();
            var body_pickup_component = collider_object.GetComponent<BodyPickupScript>();

            if (body_pickup_component) {
                GameManagerScript.instance().EnablePrompt(
                    body_pickup_component.PromptString()
                );
                prompt.type = ActivePrompt.Type.Body;
                prompt.subject = collider_object;
            } else if (pickup_component) {
                pickup_component.InvokeOnTrigger(gameObject);
                OnItemPickup(pickup_component.reward_item);
                print("Hi pickup!");
            }
        } else {
        }
    }

    void OnTriggerExit(Collider collider) {
        var collider_object = collider.gameObject;
        if (collider_object.tag == "Pickup") {
            var body_pickup_component = collider_object.GetComponent<BodyPickupScript>();
            if (body_pickup_component) {
                GameManagerScript.instance().DisablePrompt();
                prompt.type = ActivePrompt.Type.None;
            } 
        } else {
        }
    }

    void Start() {
        movement_action.started += OnMovementStart;
        drop_body.started += OnDropBody;
        turn_view.started += OnTurnStart;
        attack.started += OnAttack;
        pause_game.started += OnPauseGame;
        confirm_use.started += OnConfirmOrUse;
        open_inventory.started += OnOpenInventory;

        controller = GetComponent<GenericActorController>();
        camera = GetComponent<PunchableCamera>();

        controller.on_hurt += OnHurt;
        controller.on_death += OnDeath;

        // Should be a list of prefabs afaik
        if (items == null) items = new List<GameObject>();
        else {
            print("Already has items?");
            // sync it up with the "display" inventory
            // cause this is weird.
            foreach (GameObject existing_item in items) {
                print(existing_item.name);
                OnItemPickup(existing_item);
            }
        }
        Close3DInventory();
    }

    void OnDeath(ActorState form) {
        print("register on death");
        if (form == ActorState.Body) {
            GameManagerScript.instance().MessageLog.NewMessage(
                "You have lost your corporeal form!",
                Color.red
            );
            camera.Traumatize(0.45f);
            controller.form = ActorState.Soul;
        } else {
            GameManagerScript.instance().State = GameState.GameOver;
            pause_game.Disable();
        }
    }

    void OnHurt(int health, ActorState form) {
        // move the camera
        // soul damage is more traumatizing.
        if (form == ActorState.Soul) {
            camera.Traumatize(0.23f);
        } else {
            camera.Traumatize(0.15f);
        }
        GameManagerScript.instance().MessageLog.NewMessage("You have been hurt for " + health.ToString() + " damage!", Color.red);
    }

    void OnPauseGame(InputAction.CallbackContext ctx) {
        if (GameManagerScript.instance().State != GameState.Pause) {
            GameManagerScript.instance().State = GameState.Pause;
        } else {
            GameManagerScript.instance().State = GameState.Ingame;
        }
    }

    void OnDropBody(InputAction.CallbackContext ctx) {
        if (controller.UnequipBody()) {
            GameManagerScript.instance().MessageLog.NewMessage("Unequipping body.", Color.red);
        } else {
            GameManagerScript.instance().MessageLog.NewMessage("No body to unequip!", Color.red);
        }
    }

    void OnConfirmOrUse(InputAction.CallbackContext ctx) {
        if (InventoryActive()) {
            UseInventoryItem(selected_item_index);
            GameManagerScript.instance().InvokeNextTurn();
        } else {
            switch (prompt.type) {
                case ActivePrompt.Type.Body: {
                    print("Equipping a body.");
                    if (controller.EquipBody(prompt.subject)) {
                        GameManagerScript.instance().MessageLog.NewMessage("Equipped a new body!", Color.green);
                    } else {
                        GameManagerScript.instance().MessageLog.NewMessage("Cannot equip body while wearing one!", Color.red);
                    }
                } break;
                case ActivePrompt.Type.Activatable: {
                    print("Use activatable?");
                } break;
                default: {
                    // raycast and see what I hit.
                    RaycastHit[] hits =
                        Physics.RaycastAll(
                            transform.position,
                            transform.forward,
                            1
                        );

                    // possible bug, you can close the door on yourself?
                    foreach (RaycastHit hit in hits) {
                        var collider = hit.collider;
                        var collider_gameObject = hit.collider.gameObject;

                        // hit the trigger of the door.
                        if (collider.isTrigger && collider_gameObject.tag == "Door") {
                            var door_component = collider_gameObject.transform.parent.GetComponent<DoorScript>();
                            door_component.TryToUnlock(items);
                            door_component.UseDoor();
                            break;
                        }
                    }
                } break;
            }
            // fire a raycast and see if we hit a door?
            // if a door is locked we'll check if we have the key item I suppose.
            GameManagerScript.instance().DisablePrompt();
            prompt.type = ActivePrompt.Type.None;
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
                ((i) - selected_item_index)*1.0f,
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

    void OnOpenInventory(InputAction.CallbackContext ctx) {
        ToggleInventory();
    }

    void OnAttack(InputAction.CallbackContext ctx) {
        GameManagerScript.instance().InvokeNextTurn();
        if (active_weapon != null) {
            GameManagerScript.instance().MessageLog.NewMessage("Player attacks!", Color.white);
            var weapon_component = active_weapon.GetComponent<WeaponDataScript>();
            if (Random.Range(0f, 1f) <= hit_chance) {
                weapon_component.Attack();
            } else {
                GameManagerScript.instance().MessageLog.NewMessage("Player misses!", Color.red);
            }
            if (!weapon_component.is_ranged) {
                animator.Play("WeaponAnim", -1, 0f);
            } else {
                GameObject replica = Instantiate(GameManagerScript.instance().prefab_projectile_tracer);
                Vector3 start = transform.position + transform.right*0.125f - transform.up*0.15f;
                replica.GetComponent<LineTracerScript>().SetPoints(start, start + transform.forward * weapon_component.distance);
            }
        } else {
            GameManagerScript.instance().MessageLog.NewMessage("No weapon! Cannot attack!", Color.red);
        }
        // controller.Hurt(10);
    }

    void OnTurnStart(InputAction.CallbackContext ctx) {
        // NOTE: Turning yourself does not count as a turn action!
        // GameManagerScript.instance().InvokeNextTurn();
        if (!InventoryActive()) {
            controller.Rotate((int)ctx.ReadValue<float>());
        }

        if (controller.form == ActorState.Soul) {
            viewlight.SetActive(true);
        } else {
            viewlight.SetActive(false);
        }
    }

    void OnMovementStart(InputAction.CallbackContext ctx) {
        if (!InventoryActive()) {
            var movement = ctx.ReadValue<Vector2>();
            if (controller.MoveDirection(movement)) {
                GameManagerScript.instance().InvokeNextTurn();
            }
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
    public void EquipWeapon(GameObject weapon) {
        if (active_weapon != null) {
            Destroy(active_weapon);
        }

        // making replicas. As usual.
        active_weapon = Instantiate(weapon);
        if (active_weapon.GetComponent<CapsuleCollider>())
            active_weapon.GetComponent<CapsuleCollider>().enabled = false;
        // grr prefab can't get itself.
        // so I'll have to "sanitize the object"
        {
            // remove the replica canvas
            //..................................... Yeah I know it's dumb
            var canvas = active_weapon.transform.Find("ReplicaCanvas(Clone)");
            print(canvas);
            if (canvas) Destroy(canvas.gameObject);

            // stop spinning!
            active_weapon.GetComponent<ItemPickupGeneric>().enabled = false;
        }

        active_weapon.transform.SetParent(gameObject.transform.Find("armpivot/handmaybe"));
        active_weapon.transform.localPosition = Vector3.zero;
        active_weapon.transform.eulerAngles = new Vector3(active_weapon.transform.eulerAngles.x, 0, active_weapon.transform.eulerAngles.z);
        {
            var weapon_component = active_weapon.GetComponent<WeaponDataScript>();
            weapon_component.holder = gameObject;
        }
    }

    // TODO: destroy inventory item
    public void UseInventoryItem(int index) {
        int i = 0;
        foreach (Transform child in inventory_display_container.transform) {
            if (i == index) {
                bool used = false;
                bool destroy = false;

                var item_data = child.gameObject.GetComponent<ItemPickupGeneric>();
                var healing_component = child.gameObject.GetComponent<HealingItem>();

                if (healing_component) {
                    GameManagerScript.instance().MessageLog.NewMessage("Healed " + healing_component.amount.ToString() + " hp!", Color.green);
                    controller.Heal(healing_component.amount);
                    used = true;
                    destroy = true;
                }

                var weapon_component = child.gameObject.GetComponent<WeaponDataScript>();
                if (weapon_component)  {
                    GameManagerScript.instance().MessageLog.NewMessage("Equipping " +  item_data.description + "!", Color.green);
                    EquipWeapon(item_data.reward_item);
                    used = true;
                    destroy = false;
                }

                if (used) {
                    if (destroy) {
                        child.parent = null;
                        Destroy(child.gameObject);
                    } 
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
