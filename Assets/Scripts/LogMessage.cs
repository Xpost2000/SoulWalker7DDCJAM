using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class LogMessage : MonoBehaviour
{
    // NOTE: messages are destroyed in the order they are made
    public float lifetime = 3.0f;
    public TextMeshProUGUI text_data;
    float max_lifetime = 0.0f;
    // Start is called before the first frame update
    void Start() {
        max_lifetime = lifetime;
    }

    // Update is called once per frame
    void Update() {
        float alpha = Mathf.Clamp(lifetime/max_lifetime-(0.15f), 0.0f, 1.0f);
        
        text_data.color =
            new Color(
                text_data.color.r,
                text_data.color.g,
                text_data.color.b,
                alpha
            );
        lifetime -= Time.deltaTime;
        if (lifetime <= 0) {
            Destroy(gameObject);
        }
    }
}
