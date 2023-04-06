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

    private float MAX_DOOR_ANIM_TIME = 0.15f;

    private float anim_timer;
    private float base_angle;
    private float opening_angle;
    private DoorAnimationState anim_state;

    private Transform door_offset_hinge;

    void Start() {
        base_angle = transform.eulerAngles.y;
        opening_angle = base_angle - 90;
        anim_state = DoorAnimationState.None;
        door_offset_hinge = transform.Find("doorhingoffset");
    }

    public void UseDoor() {
        if (anim_state == DoorAnimationState.None) {
            if (opened == false) {
                opened = true;
                anim_state = DoorAnimationState.Opening;
                anim_timer = 0.0f;
            } else {
                opened = false;
                anim_state = DoorAnimationState.Closing;
                anim_timer = 0.0f;
                print("Hi! Open me up!");
            }

            GetComponent<BoxCollider>().enabled = !opened;
        }
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
