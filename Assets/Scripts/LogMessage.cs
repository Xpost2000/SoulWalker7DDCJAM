using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class LogMessage : MonoBehaviour
{
    // NOTE: messages are destroyed in the order they are made
    public float lifetime = 3.0f;
    // Start is called before the first frame update
    void Start() {}

    // Update is called once per frame
    void Update() {
        lifetime -= Time.deltaTime;
        if (lifetime <= 0) {
            Destroy(gameObject);
        }
    }
}
