using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    enum DoorAnimationState {
        None,
        Opening,
        Closing,
    };

    // Start is called before the first frame update
    public bool opened = false;

    public string required_item = null;
    public bool locked = false;

    private float MAX_DOOR_ANIM_TIME = 0.15f;

    private float anim_timer;
    private float base_angle;
    private float opening_angle;
    private DoorAnimationState anim_state;

    private Transform door_offset_hinge;
    public GameObject soundsource;

    void Start() {
        base_angle = transform.eulerAngles.y;
        opening_angle = base_angle - 90;
        anim_state = DoorAnimationState.None;
        door_offset_hinge = transform.Find("doorhingoffset");
    }

    public void TryToUnlock(List<GameObject> items) {
        if (locked) {
            foreach (GameObject item in items) {
                if (item.name == required_item) {
                    locked = false;
                    GameManagerScript.instance().MessageLog.NewMessage(
                        "Door unlocked with " + required_item, Color.green
                    );
                    return;
                } else {
                    print(item.name);
                    print("But I need " +  required_item);
                }
            }
            GameManagerScript.instance().MessageLog.NewMessage(
                "Door cannot be unlocked!", Color.red);
        }
    }

    public void OpenDoor() {
        opened = true;
        anim_state = DoorAnimationState.Opening;
        anim_timer = 0.0f;
        GetComponent<BoxCollider>().enabled = !opened;
        soundsource.GetComponent<AudioSource>().Play();
    }

    public void LockDoor() {
        opened = false;
        anim_state = DoorAnimationState.Closing;
        anim_timer = 0.0f;
        GetComponent<BoxCollider>().enabled = !opened;
        soundsource.GetComponent<AudioSource>().Play();
    }

    public void ForceUseDoor() {
        if (anim_state == DoorAnimationState.None) {
            if (opened == false) {
                OpenDoor();
            } else {
                LockDoor();
            }
        }
    }

    public void UseDoor() {
        if (locked) {
            GameManagerScript.instance().MessageLog.NewMessage(
                "Door is locked!", Color.red
            );
            return;
        }

        ForceUseDoor();
    }

    // Update is called once per frame
    // play sound?
    void Update() {
        float dt = Time.deltaTime;

        switch (anim_state) {
            case DoorAnimationState.None: {
                // none
                float angle = 0.0f;
                if (opened) angle = opening_angle;
                else        angle = base_angle;

                door_offset_hinge.eulerAngles = new Vector3(
                    door_offset_hinge.eulerAngles.x,
                    angle,
                    door_offset_hinge.eulerAngles.z
                );
            } break;
            case DoorAnimationState.Opening: {
                anim_timer += dt;
                float effective_t = anim_timer / MAX_DOOR_ANIM_TIME;

                if (anim_timer >= MAX_DOOR_ANIM_TIME) {
                    anim_state = DoorAnimationState.None;
                }

                door_offset_hinge.eulerAngles = new Vector3(
                    door_offset_hinge.eulerAngles.x,
                    Mathf.Lerp(base_angle, opening_angle, effective_t),
                    door_offset_hinge.eulerAngles.z
                );
            } break;
            case DoorAnimationState.Closing: {
                anim_timer += dt;

                float effective_t = anim_timer / MAX_DOOR_ANIM_TIME;

                if (anim_timer >= MAX_DOOR_ANIM_TIME) {
                    anim_state = DoorAnimationState.None;
                }

                transform.eulerAngles = new Vector3(
                    door_offset_hinge.eulerAngles.x,
                    Mathf.Lerp(opening_angle, base_angle, effective_t),
                    door_offset_hinge.eulerAngles.z
                );
            } break;
        }
    }
}
