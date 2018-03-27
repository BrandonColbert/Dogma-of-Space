using UnityEngine;
using System.Collections;

public class ShipController : MonoBehaviour {
    public Ship ship;

    void FixedUpdate() {
        Move(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    void Move(float x, float y) {
		Rigidbody2D shipBody = Ship.gameObject.GetComponent<Rigidbody2D>();
        
        Vector2 forceVector = new Vector2(x, y) * Ship.speed * Time.deltaTime;
        
        shipBody.velocity = forceVector;
    }
}