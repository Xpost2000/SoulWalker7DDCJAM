using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class UIGameplayScript : MonoBehaviour
{
    public GameObject player_object;
    public GameObject hp_text_target;
    public GameObject existence_text_target;

    void UpdateHPText() {
        var player_actor_controller = player_object.GetComponent<GenericActorController>();
        { // Health depends on targets.
            var text = hp_text_target.GetComponent<TextMeshProUGUI>();
            text.text = player_actor_controller.Health.ToString();
            float health_percentage = player_actor_controller.HealthPercent;

            if (health_percentage < 0.25) {
                text.color = Color.red;
            } else if (health_percentage < 0.5) {
                text.color = Color.yellow;
            } else {
                text.color = Color.green;
            }
        }
        {
            var text = existence_text_target.GetComponent<TextMeshProUGUI>();
            switch (player_actor_controller.form) {
                case ActorState.Soul: {
                    text.text = "SOUL FORM";
                    text.color = Color.yellow;
                } break;
                case ActorState.Body: {
                    text.text = "BODY FORM";
                    text.color = Color.white;
                } break;
            }
        }
    }

    void Start() {
        UpdateHPText();
    }

    void Update() {
        UpdateHPText();
    }
}
