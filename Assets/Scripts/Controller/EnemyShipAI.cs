using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyShipAI : FighterController {
    public enum Act {
        WANDER,
        CHASE,
        ATTACK,
        EVADE
    }

    [Tooltip("Ship to attack")]
    public Ship target;
    [Tooltip("Course of action")]
    public Act directive;
    [Tooltip("Distance for a ship to become a valid target")]
    public float followRange = 200f;
    [Tooltip("Distance before a target can be shot at")]
    public float attackRange = 30f;
    [Tooltip("Distance before a target can be shot at")]
    public float minAttackAngle = 15f;
    [Tooltip("Minimum angle between the ship's heading and target to attack")]
    public float attackInaccuracy = 5f;
    [Tooltip("Minimum distance to keep between a target while attacking")]
    public float minDistance = 2f;
    [Tooltip("Maxmimum distance to keep between a target while attacking")]
    public float maxDistance = 5f;
    [Tooltip("Percent of health at which to begin fleeing")]
    [Range(0f, 1f)] public float fearPercent = 0.15f;

    private bool changingDirection;
    private float speed;
    private float targetAngle, angleBetween;
    private bool rotatingTowards;

    public override void Logic(Fighter fighter) {
        if(target == null) {
            List<Ship> targets = new List<Ship>();

            foreach(Ship ship in Ship.ships) {
                if(ship.controller && ship.controller is PlayerFighterController) {
                    if(Vector3.Distance(fighter.transform.position, ship.transform.position) < followRange) {
                        targets.Add(ship);
                    }
                }
            }

            if(targets.Count > 0) {
                target = targets[MathHelper.Rand(0, targets.Count - 1)];
            }
        } else if(Vector3.Distance(fighter.transform.position, target.transform.position) > followRange) {
            target = null;
        }

        if(target) {
            if(fighter.attributes.shields <= 0f && fighter.attributes.health / fighter.attributes.maxHealth < fearPercent) {
                directive = Act.EVADE;
            } else if(Vector2.Distance(fighter.transform.position, target.transform.position) < attackRange) {
                directive = Act.ATTACK;
            } else {
                directive = Act.CHASE;
            }
        } else {
            directive = Act.WANDER;
        }
    }

    public override void PhysicsLogic(Fighter fighter) {
        switch(directive) {
            case Act.ATTACK:
                if(target) {
                    Vector2 dif = target.transform.position - transform.position;
                    float angle = Angle(dif) - 90f;
                    
                    if(dif.magnitude > maxDistance) {
                        fighter.Move(1);
                    } else if(dif.magnitude < minDistance) {
                        fighter.Move(-1);
                    } else {
                        fighter.Move(0);
                    }

                    if(Vector3.Distance(fighter.transform.position, target.transform.position) < attackRange && (angleBetween < minAttackAngle || fighter.CanBeAimed())) {
                        Vector3 inaccuracy = new Vector3(MathHelper.Rand(-attackInaccuracy, attackInaccuracy), MathHelper.Rand(-attackInaccuracy, attackInaccuracy));
                        angle += MathHelper.Rand(-attackInaccuracy, attackInaccuracy);

                        fighter.Aim(fighter.ReadyToFire() ? target.transform.position + inaccuracy : target.transform.position);
                        fighter.Fire();
                    }

                    foreach(Ship ship in Ship.ships) {
                        if(ship && Vector2.Distance(transform.position, ship.transform.position) < followRange && ship.shipID != fighter.shipID && (target == null || ship.shipID != target.shipID)) {
                            angle -= Vector2.SignedAngle(ship.transform.position, transform.position) / Mathf.Clamp(1f, (ship.transform.position - transform.position).sqrMagnitude, followRange);
                        }
                    }

                    RotateTowards(fighter, angle);
                }
                break;
            case Act.CHASE:
                if(target) {
                    RotateTowards(fighter, Angle(target.transform.position - transform.position) - 90f);
                    if(angleBetween < minAttackAngle) fighter.Move(1);
                }
                break;
            case Act.EVADE:
                if(target) {
                    RotateTowards(fighter, Angle(target.transform.position - transform.position) + 90f);
                    fighter.Move(1);
                }
                break;
            case Act.WANDER:
                fighter.Move(speed);

                if(!changingDirection) {
                    speed = MathHelper.Rand(0.25f, 1f);
                    StartCoroutine(ChangeDirection(fighter));
                }
                break;
        }
    }

    public float Angle(Vector2 dif) {
        return Mathf.Atan2(dif.y, dif.x) * Mathf.Rad2Deg;
    }

    public void RotateTowards(Fighter fighter, float angle) {
        targetAngle = angle;
        if(!rotatingTowards) StartCoroutine(CRotateTowards(fighter));
    }

    IEnumerator CRotateTowards(Fighter fighter) {
        rotatingTowards = true;

        float current, rate, dest;

        while((current = fighter.transform.eulerAngles.z) != targetAngle) {
            rate = fighter.attributes.handling * Time.deltaTime;

            float dl = current + rate, dr = current - rate;
            float t = Mathf.Abs(Mathf.DeltaAngle(current, targetAngle)), tl = Mathf.Abs(Mathf.DeltaAngle(dl, targetAngle)), tr = Mathf.Abs(Mathf.DeltaAngle(dr, targetAngle));

            bool clockwise = tl < tr;

            if((clockwise ? tl : tr) < t) {
                dest = clockwise ? dl : dr;
            } else {
                dest = targetAngle;
            }

            fighter.transform.eulerAngles = new Vector3(0, 0, dest);

            angleBetween = Mathf.Abs(Mathf.DeltaAngle(fighter.transform.eulerAngles.z, targetAngle));

            yield return null;
        }

        rotatingTowards = false;
    }

    IEnumerator ChangeDirection(Fighter fighter) {
        changingDirection = true;

        RotateTowards(fighter, MathHelper.Rand(0f, 360f));
        yield return new WaitForSeconds(MathHelper.Rand(5, 30));

        changingDirection = false;
    }
}