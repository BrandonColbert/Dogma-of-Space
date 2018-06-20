using UnityEngine;

public class FighterWeapon : MonoBehaviour {
    [HideInInspector]
    public Ship ship;

    public GameObject projectile;
    public Vector2 fireLocation;
    public Vector2 fireDirection;
    public bool aimable, hitscan, homing;
    public float fireDamage = 1f, fireRate = 15f, projectileSpeed = 20f;

    public TimerTask timer = new TimerTask();

    public bool Fire() {
        if(gameObject.activeSelf) {
            if(timer.Next(1 / fireRate)) {
                GameObject shellObject = Instantiate(projectile, gameObject.transform.position, transform.rotation);
                shellObject.transform.Translate(fireLocation);

                if(aimable) {
                    shellObject.transform.Rotate(Vector3.forward,
                        90f -
                        gameObject.transform.eulerAngles.z +
                        Mathf.Rad2Deg * Mathf.Atan2(gameObject.transform.position.y - fireDirection.y, gameObject.transform.position.x - fireDirection.x)
                    );
                }

                Shell shell = shellObject.AddComponent<Shell>();
                shell.source = ship;

                if(hitscan) {
                    HitscanShot(shell);
                } else {
                    shell.speed = ship.GetComponent<Rigidbody2D>().velocity + (Vector2)shell.transform.up * projectileSpeed;
                    shell.movementLogic = delegate(Shell s, Rigidbody2D rb) { ShellMovementLogic(s, rb); };
                    shell.collisionLogic = delegate(Shell s, GameObject collided, bool isShip, Collision2D collision) { ShellCollisionLogic(s, collided, isShip, collision); };
                }

                return true;
            }
        }

        return false;
    }

    public virtual void ShellMovementLogic(Shell shell, Rigidbody2D rb) {
        rb.velocity = shell.speed;

        if(Vector3.Distance(shell.transform.position, transform.position) > 100f) {
            Destroy(shell.gameObject);
        }
    }

    public virtual void ShellCollisionLogic(Shell shell, GameObject collided, bool isShip, Collision2D collision) {
        if(isShip) {
            if(collided.GetComponent<Ship>().shipID == shell.source.shipID) {
                Physics2D.IgnoreCollision(collision.collider, collision.otherCollider);
            } else {
                collided.GetComponent<Ship>().Damage(fireDamage);
                Destroy(shell.gameObject);
            }
        } else if(collided.GetComponent<Shell>() != null) {
            if(collided.GetComponent<Shell>().source.shipID == shell.source.shipID) {
                Physics2D.IgnoreCollision(collision.collider, collision.otherCollider);
            } else {
                Destroy(collided);
                Destroy(shell.gameObject);
            }
        } else if(collided.GetComponent<BreakableObject>() != null) {
            Vector3 impactPoint = shell.transform.position - collided.transform.position;
            Vector3 finalPoint = impactPoint + (Vector3)shell.speed;

            collided.GetComponent<BreakableObject>().Shatter(impactPoint, finalPoint, shell.speed.magnitude * 100 * shell.GetComponent<Rigidbody2D>().mass, fireDamage);

            Destroy(shell.gameObject);
        } else {
            Destroy(shell.gameObject);
        }
    }

    public virtual void HitscanShot(Shell shell) {
        Destroy(shell.gameObject);
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)fireLocation);
        Gizmos.DrawSphere(transform.position + (Vector3)fireLocation, 0.01f);
    }
}