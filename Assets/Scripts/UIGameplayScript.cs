using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class UIGameplayScript : MonoBehaviour
{
    public GameObject fader;

    public GameObject exposition_tutorial_panel;
    public GameObject exposition_tutorial_panel_text;

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

    /*
      These are mainly excuses to use coroutines so I just know how to use them.

      They're sorta neat I suppose.
     */
    IEnumerator _OpenExpositionTutorialPanel(string message, float read_speed, float delaytime) {
        int text_length = 0;
        var panel_graphic = exposition_tutorial_panel.GetComponent<Image>();
        var text_object = exposition_tutorial_panel_text.GetComponent<TextMeshProUGUI>();

        exposition_tutorial_panel.SetActive(true);
        GameManagerScript.instance().player.GetComponent<PlayerController>().DisableInput();

        // fade in
        for (float t = 0; t < 0.5f; t += Time.deltaTime) {
            Color c = panel_graphic.color;
            c.a = t / 0.5f;
            panel_graphic.color = c;
            yield return null;
        }

        // type message
        while (text_length <= message.Length) {
            text_object.text = message.Substring(0, text_length);
            yield return new WaitForSeconds(read_speed);
            text_length++;
        }
        text_length--;
        yield return new WaitForSeconds(delaytime);
        while (text_length >= 0) {
            text_object.text = message.Substring(0, text_length);
            yield return new WaitForSeconds(read_speed);
            text_length--;
        }

        // fade out
        for (float t = 0; t < 0.5f; t += Time.deltaTime) {
            Color c = panel_graphic.color;
            c.a = 1 - (t / 0.5f);
            panel_graphic.color = c;
            yield return null;
        }

        exposition_tutorial_panel.SetActive(false);
        GameManagerScript.instance().player.GetComponent<PlayerController>().EnableInput();
        yield break;
    }

    public void OpenExpositionTutorialPanel(string message, float read_speed=0.05f, float delaytime=1.0f) {
        StartCoroutine(
            _OpenExpositionTutorialPanel(message, read_speed, delaytime)
        );
    }

    void Start() {
        {
            var player_actor_controller = player_object.GetComponent<GenericActorController>();
            player_actor_controller.on_hurt += delegate(int amount, ActorState form) { StartCoroutine(AnimateFader(Color.red, 0.2f, 0.15f)); };
        }
        {
            var player_actor_controller = player_object.GetComponent<PlayerController>();
            player_actor_controller.on_heal += delegate() { StartCoroutine(AnimateFader(Color.green, 0.2f, 0.15f)); };
            player_actor_controller.on_body_equip += delegate() { StartCoroutine(AnimateFader(Color.yellow, 0.2f, 0.15f)); };
        }
        UpdateHPText();
        DisablePrompt();
    }

    void Update() {
        UpdateHPText();
    }
}
