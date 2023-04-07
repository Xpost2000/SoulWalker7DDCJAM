using UnityEngine;

public class ExpositionTutorialPromptOpenerScript : MonoBehaviour {
    [TextArea]
    public string text;
    public float readspeed = 0.05f;
    public float delaytime = 1.0f;

    void Start() {}
    void Update() {}

    void OnTriggerEnter(Collider collider) {
        var collider_gameobject = collider.gameObject;

        if (collider_gameobject.tag == "Player") {
            GameManagerScript.instance().OpenExpositionTutorialPanel(
                text,
                readspeed,
                delaytime
            );

            // destroy self, no longer needed!
            // I hope.
            Destroy(gameObject);
        }
    }
}
