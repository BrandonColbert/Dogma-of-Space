using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Ship : MonoBehaviour {
    public float maxHealth = 100f;
    public float health = 0f;
    
    public float speed = 250f;

    [HideInInspector]
    public float previousHealth = 0f;

    void Start() {
        Spawn();
    }

    public void Spawn() {
        health = maxHealth;
        GetComponent<Rigidbody2D>().gravityScale = 0;
	}

    public void Heal(float value) {
        previousHealth = health;
        health += value;
    }

    public void Damage(float value) {
        previousHealth = health;
        health -= value;

        if(health < 0) {
            Destroy(gameObject);
        }
    }
}
