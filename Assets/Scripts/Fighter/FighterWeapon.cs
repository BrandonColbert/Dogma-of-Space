using System;
using UnityEngine;

public class FighterWeapon : MonoBehaviour {
    [HideInInspector]
    public Ship ship;

    public GameObject projectile;
    public ParticleSystem particleEffect;
    public Vector2 fireLocation;
    public Vector2 fireDirection;
    public float maxShellDistance = 250f;
    public bool aimable, hitscan, homing, pierces;
    public float fireDamage = 1f, fireRate = 15f, projectileSpeed = 20f;

    public TimerTask timer = new TimerTask();

    void Start() {
        UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);
    }

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
                shell.speed = ship.GetComponent<Rigidbody2D>().velocity + (Vector2)shell.transform.up * projectileSpeed;
                shell.movementLogic = delegate(Shell s, Rigidbody2D rb) { ShellMovementLogic(s, rb); };

                if(hitscan) {
                    if(shell.GetComponent<Collider2D>()) Destroy(shell.GetComponent<Collider2D>());
                    HitscanShot(shell);
                } else {
                    shell.collisionLogic = delegate(Shell s, GameObject collided, bool isShip, Collision2D collision) { ShellCollisionLogic(s, collided, isShip, collision); };
                }

                if(particleEffect != null) {
                    particleEffect.Emit(UnityEngine.Random.Range(10, 20));
                }

                return true;
            }
        }

        return false;
    }

    public virtual void ShellMovementLogic(Shell shell, Rigidbody2D rb) {
        rb.velocity = shell.speed;

        if(this) {
            if(hitscan) {
                if(Vector2.Distance(transform.position, shell.transform.position) > shell.travelDistance) {
                    Destroy(shell.gameObject);
                }
            } else {
                if(Vector3.Distance(shell.transform.position, transform.position) > maxShellDistance) {
                    Destroy(shell.gameObject);
                }
            }
        } else {
            Destroy(shell.gameObject);
        }
    }

    public virtual void ShellCollisionLogic(Shell shell, GameObject collided, bool isShip, Collision2D collision) {
        if(isShip) {
            if(collided.GetComponent<Ship>().shipID == shell.source.shipID) {
                Physics2D.IgnoreCollision(collision.collider, collision.otherCollider);
            } else {
                collided.GetComponent<Ship>().Damage(fireDamage);
                if(!pierces) Destroy(shell.gameObject);
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

            if(!pierces) Destroy(shell.gameObject);
        } else {
            Destroy(shell.gameObject);
        }
    }

    public virtual void HitscanShot(Shell shell) {
        Action<Transform> enact = delegate(Transform hit) {
            if(hit.GetComponent<Ship>()) {
                hit.GetComponent<Ship>().Damage(fireDamage);
            } else if(hit.GetComponent<BreakableObject>()) {
                Vector3 impactPoint = shell.transform.position - hit.position;
                Vector3 finalPoint = impactPoint + (Vector3)shell.speed;

                hit.GetComponent<BreakableObject>().Shatter(impactPoint, finalPoint, shell.speed.magnitude * 100 * shell.GetComponent<Rigidbody2D>().mass, fireDamage);
            }
        };

        shell.travelDistance = maxShellDistance;
        RaycastHit2D[] hits = Physics2D.RaycastAll(shell.transform.position, shell.transform.up, maxShellDistance);

        if(hits.Length > 0) {
            if(pierces) {
                foreach(RaycastHit2D hit in hits) {
                    if(!hit.transform.gameObject.GetComponent<Shell>() && (!hit.transform.GetComponent<Ship>() || hit.transform.GetComponent<Ship>().shipID != ship.shipID)) {
                        enact(hit.transform);
                    }
                }
            } else {
                Transform closest = null;

                foreach(RaycastHit2D hit in hits) {
                    if(!hit.transform.gameObject.GetComponent<Shell>() && (!hit.transform.GetComponent<Ship>() || hit.transform.GetComponent<Ship>().shipID != ship.shipID)) {
                        if(!closest || Vector2.Distance(shell.transform.position, hit.transform.position) < Vector2.Distance(shell.transform.position, closest.transform.position)) {
                            closest = hit.transform;
                        }
                    }
                }

                if(closest) {
                    shell.travelDistance = Vector2.Distance(transform.position, closest.position);
                    enact(closest);
                }
            }
        }
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.blue;

        Vector3 end = transform.position + MathHelper.RotateAround((Vector3)fireLocation, Vector3.zero, transform.rotation);
        Gizmos.DrawLine(transform.position, end);
        Gizmos.DrawSphere(end, 0.01f);
    }
}