using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DamageSource : MonoBehaviour {
    public float damageFactor = 1f;

    void OnCollisionEnter2D(Collision2D collision) {
        Ship ship = collision.collider.gameObject.GetComponent<Ship>();

        if(ship != null) {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            Rigidbody2D orb = ship.gameObject.GetComponent<Rigidbody2D>();

            float damage = (rb.velocity * rb.mass - orb.velocity * orb.mass).magnitude * damageFactor;
            ship.Damage(damage);
        }
    }
}