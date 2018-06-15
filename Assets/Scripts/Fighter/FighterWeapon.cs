using UnityEngine;

public class FighterWeapon : MonoBehaviour {
    [HideInInspector]
    public Ship ship;

    public GameObject projectile;
    public Vector3 fireLocation;
    public bool aimable, hitscan, homing;
    public float fireDamage = 1f, fireRate = 15f, projectileSpeed = 20f;

    private TimerTask timer = new TimerTask();

    public void Fire() {
        if(gameObject.activeSelf) {
            if(timer.Ready(1 / fireRate)) {
                GameObject shellObject = Instantiate(projectile, gameObject.transform.position, transform.rotation);
                shellObject.transform.Translate(fireLocation);

                if(aimable) {
                    Vector3 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                    shellObject.transform.Rotate(Vector3.forward,
                        90f -
                        gameObject.transform.eulerAngles.z +
                        Mathf.Rad2Deg * Mathf.Atan2(gameObject.transform.position.y - direction.y, gameObject.transform.position.x - direction.x)
                    );
                }

                Shell shell = shellObject.AddComponent<Shell>();
                shell.source = ship;

                if(hitscan) {
                    HitscanShot(shell);
                } else {
                    shell.speed = projectileSpeed + ship.GetComponent<Rigidbody2D>().velocity.magnitude;
                    shellObject.AddComponent<Rigidbody2D>().mass = 0.05f;//0.0001f;
                    shell.movementLogic = delegate(Shell s, Rigidbody2D rb) { ShellMovementLogic(s, rb); };
                    shell.collisionLogic = delegate(Shell s, GameObject collided, bool isShip, Collision2D collision) { ShellCollisionLogic(s, collided, isShip, collision); };
                }
            }
        }
    }

    public virtual void ShellMovementLogic(Shell shell, Rigidbody2D rb) {
        rb.velocity = shell.transform.up * shell.speed;

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
            Vector3 finalPoint = impactPoint + shell.transform.up * shell.speed;

            collided.GetComponent<BreakableObject>().Shatter(impactPoint, finalPoint, projectileSpeed * 2f, fireDamage);

            Destroy(shell.gameObject);
        } else {
            Destroy(shell.gameObject);
        }
    }

    public virtual void HitscanShot(Shell shell) {
        Destroy(shell.gameObject);
    }
}