using UnityEngine;
using System.Collections;

public class EnemyShipAI : MonoBehaviour {
    public Ship ship;
    public bool direction;

    void Start() {

    }
    
    void FixedUpdate() {
        Rigidbody2D rb = ship.GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(0, ship.speed) * (direction ? 1 : -1);

        if(ship is Fighter) {
            Fighter fighter = ship as Fighter;
            fighter.FireShell(true);
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.GetComponent<Shell>() == null) direction = !direction;
    }
}
