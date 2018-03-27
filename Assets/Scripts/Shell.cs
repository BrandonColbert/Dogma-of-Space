using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour {
	public int damage;
	public bool homing;

    void Start() {
        gameObject.name = "Shell";
    }

    void FixedUpdate() {

	}

	void OnCollisionEnter2D(Collision2D collision) {
        Ship ship = collision.gameObject.GetComponent<Ship>();

		if(ship != null) {
            ship.Damage(damage);
		}

        Destroy(gameObject);
    }
}