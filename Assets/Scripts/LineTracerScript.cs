using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineTracerScript : MonoBehaviour
{
    public LineRenderer line_renderer;
    public ParticleSystem particle_system;
    // Start is called before the first frame update
    public float particle_distance = 0.5f;
    public float tracer_lifetime = 0.45f;
    void Start() {}

    // Update is called once per frame
    void Update() {
        float dt = Time.deltaTime;
        tracer_lifetime -= dt;
        if (tracer_lifetime <= 0.0f) {
            Destroy(gameObject);
        }
    }

    public void SetPoints(Vector3 start, Vector3 end) {
        if (particle_system != null) {
            var particle_shape_module = particle_system.shape;
            particle_shape_module.radius = particle_distance; 
            particle_system.transform.position = start;
            particle_system.transform.LookAt(end);
        }

        line_renderer.positionCount = 2;
        line_renderer.SetPosition(0, start);
        line_renderer.SetPosition(1, end);
    }
}
