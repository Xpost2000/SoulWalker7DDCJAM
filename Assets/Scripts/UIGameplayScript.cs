using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class UIGameplayScript : MonoBehaviour
{
    public GameObject player_object;
    public GameObject hp_text_target;

    void UpdateHPText() {
        var text = hp_text_target.GetComponent<TextMeshProUGUI>();
        var player_actor_controller = player_object.GetComponent<GenericActorController>();
        text.text = player_actor_controller.health.ToString();

        float health_percentage = (player_actor_controller.health/player_actor_controller.max_health);

        if (health_percentage < 0.25) {
            text.color = Color.red;
        } else if (health_percentage < 0.5) {
            text.color = Color.yellow;
        }
    }

    void Start() {
        UpdateHPText();
    }

    void Update() {
        UpdateHPText();
    }
}
