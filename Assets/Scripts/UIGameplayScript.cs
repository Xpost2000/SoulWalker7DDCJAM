using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class UIGameplayScript : MonoBehaviour
{
    public GameObject fader;
    public GameObject player_object;
    public GameObject hp_text_target;
    public GameObject existence_text_target;
    public GameObject display_prompt_target;

    public void EnablePrompt(string settext) {
        display_prompt_target.SetActive(true);
        var text = display_prompt_target.GetComponent<TextMeshProUGUI>();
        text.text = settext;
    }

    public void DisablePrompt() {
        display_prompt_target.SetActive(false);
    }

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

    IEnumerator AnimateFader(Color color, float maxtime, float delaytime) {
        var image = fader.GetComponent<Image>();

        float fade_in_t = 0;

        image.color = color;
        while (fade_in_t < maxtime/2.0f) {
            fade_in_t += Time.deltaTime;
            Color c = image.color;
            c.a = fade_in_t/(maxtime/2.0f); 
            image.color = c;
            yield return null;
        }
        yield return new WaitForSeconds(delaytime);

        float fade_out_t = 0;
        while (fade_out_t < maxtime/2.0f) {
            fade_out_t += Time.deltaTime;
            Color c = image.color;
            c.a = 1 - fade_out_t/(maxtime/2.0f);
            image.color = c;
            yield return null;
        }

        yield break;
    }

    void Start() {
        {
            var player_actor_controller = player_object.GetComponent<GenericActorController>();
            player_actor_controller.on_hurt += delegate(int amount, ActorState form) { StartCoroutine(AnimateFader(Color.red, 0.4f, 0.15f)); };
        }
        {
            var player_actor_controller = player_object.GetComponent<PlayerController>();
            player_actor_controller.on_heal += delegate() { StartCoroutine(AnimateFader(Color.green, 0.4f, 0.15f)); };
            player_actor_controller.on_body_equip += delegate() { StartCoroutine(AnimateFader(Color.yellow, 0.4f, 0.15f)); };
        }
        UpdateHPText();
    }

    void Update() {
        UpdateHPText();
    }
}
