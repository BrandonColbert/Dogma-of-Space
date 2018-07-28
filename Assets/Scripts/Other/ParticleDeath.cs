using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleDeath : MonoBehaviour {
    private ParticleSystem particles;

    void Start() {
        particles = GetComponent<ParticleSystem>();
    }

    void Update() {
        if(!particles.IsAlive()) Destroy(gameObject);
    }
}