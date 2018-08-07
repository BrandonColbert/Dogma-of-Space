using UnityEngine;

public class ImpactDamageSource : MonoBehaviour {
    public float damageFactor = 1f;

    void OnCollisionEnter2D(Collision2D collision) {
        GameObject obj = collision.collider.gameObject;

        if(obj) {
            Damageable objDMG = obj.GetComponent<Damageable>();
            Rigidbody2D objRB = obj.GetComponent<Rigidbody2D>();

            if(objDMG && objRB) {
                float acceleration = (objRB.velocity - objDMG.lastVelocity).magnitude / Time.fixedDeltaTime;

                BreakableObject br = obj.GetComponent<BreakableObject>();
                if(br) {
                    if(br.GetArea() < 5f) {
                        return;
                    } else {
                        Rigidbody2D rb = GetComponent<Rigidbody2D>();
                        if(rb) {
                            Vector3 impactPoint = transform.position - collision.collider.transform.position;
                            Vector3 finalPoint = impactPoint + (Vector3)rb.velocity;
                            br.Adjust(impactPoint, finalPoint, acceleration * objRB.mass);
                        }
                    }
                }

                float damage = acceleration / objRB.mass * damageFactor;

                if(damage >= 1f) {
                    //Debug.Log(name + " damaged " + objDMG.name + " by " + damage);
                    objDMG.Damage(damage);
                }
            }
        }
    }
}