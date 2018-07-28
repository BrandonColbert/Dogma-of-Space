using UnityEngine;
using System.Collections;

public class EnemyShipAI : FighterController {
    public Ship target;
    public float targetFollowRange = 150f;
    public float targetAttackRange = 30f;
    public float attackInaccuracy = 5f;
    public float minDistance = 2f;
    public float maxDistance = 5f;

    public override void Logic(Fighter fighter) {
        if(target == null) {
            foreach(Ship ship in Ship.ships) {
                if(ship.controller && ship.controller is PlayerFighterController) {
                    float shipDistance = Vector3.Distance(fighter.transform.position, ship.transform.position);
                    if(shipDistance <= targetFollowRange) {
                        target = target == null ? ship : (shipDistance < Vector3.Distance(fighter.transform.position, target.transform.position) ? ship : target);
                    }
                }
            }
        } else if(Vector3.Distance(fighter.transform.position, target.transform.position) > targetFollowRange) {
            target = null;
        }
    }

    public override void PhysicsLogic(Fighter fighter) {
        if(target) {
            if(randomlyTurning) StopCoroutine(changeDirectionC);
            randomlyTurning = false;

            //float angle = fighter.transform.eulerAngles.z;

            Vector2 dif = target.transform.position - transform.position;
            float targetAngle = Mathf.Atan2(dif.y, dif.x) * Mathf.Rad2Deg - 90f;
            
            if(dif.magnitude > maxDistance) {
                fighter.Move(1);
            } else if(dif.magnitude < minDistance) {
                fighter.Move(-1);
            } else {
                fighter.Move(0);
            }

            if(Vector3.Distance(fighter.transform.position, target.transform.position) <= targetAttackRange) {
                Vector3 inaccuracy = new Vector3(MathHelper.Rand(-attackInaccuracy, attackInaccuracy), MathHelper.Rand(-attackInaccuracy, attackInaccuracy));
                targetAngle += MathHelper.Rand(-attackInaccuracy, attackInaccuracy);

                fighter.Aim(fighter.ReadyToFire() ? target.transform.position + inaccuracy : target.transform.position);
                fighter.Fire();
            }

            foreach(Ship ship in Ship.ships) {
                if(Vector2.Distance(transform.position, ship.transform.position) < targetFollowRange && ship.shipID != fighter.shipID && (target == null || ship.shipID != target.shipID)) {
                    targetAngle -= Vector2.SignedAngle(ship.transform.position, transform.position) / Mathf.Clamp(1f, (ship.transform.position - transform.position).sqrMagnitude, targetFollowRange);
                }
            }

            fighter.transform.eulerAngles = new Vector3(0, 0, targetAngle);
        } else {
            fighter.Move(1);

            if(!randomlyTurning) {
                randomlyTurning = true;
                changeDirectionC = ChangeDirection(fighter);
                StartCoroutine(changeDirectionC);
            }
        }
    }

    private bool randomlyTurning;
    private IEnumerator changeDirectionC; 
    IEnumerator ChangeDirection(Fighter fighter) {
        while(randomlyTurning) {
            fighter.transform.eulerAngles = new Vector3(0, 0, MathHelper.Rand(0f, 360f));
            yield return new WaitForSeconds(MathHelper.Rand(5, 30));
        }
    }
}