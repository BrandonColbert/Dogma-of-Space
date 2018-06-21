using System;
using UnityEngine;

public class Shell : MonoBehaviour {
    public Ship source;
    public Vector2 speed;
    public float travelDistance;
    public Action<Shell, Rigidbody2D> movementLogic;
    public Action<Shell, GameObject, bool, Collision2D> collisionLogic;
    
    private Rigidbody2D rb;

    void Start() {
        gameObject.name = "Shell";
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
    }

    void FixedUpdate() {
        if(movementLogic != null) {
            movementLogic(this, rb);
        }
	}

	void OnCollisionEnter2D(Collision2D collision) {
        if(collisionLogic != null) {
            collisionLogic(this, collision.gameObject, collision.gameObject.GetComponent<Ship>() != null, collision);
        }
    }
}