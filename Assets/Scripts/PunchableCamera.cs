using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchableCamera : MonoBehaviour
{
    /*
      usually my trauma cameras affect position or otherwise
      stuff that has information that matters...

      But since we're only jittering the camera's rotation on an
      axis I don't use. This is way easier at least.
    */

    public float trauma_limit = 1.0f;
    private float trauma = 0.0f;

    public Camera camera;

    void Start() {
        
    }

    public void Traumatize(float amount) {
        trauma += amount;
        if (trauma >= trauma_limit) trauma = trauma_limit;
    }

    // Update is called once per frame
    void Update() {
        float trauma_offset_angle = Random.Range(-90.0f, 90.0f) * trauma * 0.2f;
        float trauma_offset_angle1 = Random.Range(-90.0f, 90.0f) * trauma * 0.85f;
        float dt = Time.deltaTime;

        camera.transform.eulerAngles = new Vector3(
            trauma_offset_angle,
            camera.transform.eulerAngles.y,
            trauma_offset_angle1
        );

        trauma -= dt * 0.198f;
        if (trauma <= 0.0f) trauma = 0.0f;
    }
}
