using UnityEngine;
using System.Collections;

public class EnemyShipAI : FighterController {
    public Ship target;
    public float targetRange = 50f;
    public float minDistance = 2f;
    public float maxDistance = 8f;

    public override void Logic(Fighter fighter) {
        if(target == null) {
            foreach(Ship ship in Ship.ships) {
                if(ship.controller && ship.controller is PlayerFighterController) {
                    float shipDistance = Vector3.Distance(fighter.transform.position, ship.transform.position);
                    if(shipDistance <= targetRange) {
                        target = target == null ? ship : (shipDistance < Vector3.Distance(fighter.transform.position, target.transform.position) ? ship : target);
                    }
                }
            }
        } else if(Vector3.Distance(fighter.transform.position, target.transform.position) > targetRange) {
            target = null;
        }
    }

    public override void PhysicsLogic(Fighter fighter) {
        float angle = fighter.transform.eulerAngles.z;
        float targetAngle = 0f;

        if(target) {
            Vector2 dif = target.transform.position - transform.position;
            targetAngle = Mathf.Atan2(dif.y, dif.x) * Mathf.Rad2Deg - 90f;
            
            if(dif.magnitude > maxDistance) {
                fighter.Move(1);
            } else if(dif.magnitude < minDistance) {
                fighter.Move(-1);
            } else {
                fighter.Move(0);
            }

            fighter.Aim(target.transform.position);
            fighter.Fire();
        } else {
            targetAngle = angle;
            fighter.Move(1);
            targetAngle += MathHelper.Rand(-3f, 3f);
        }

        foreach(Ship ship in Ship.ships) {
            if(Vector2.Distance(transform.position, ship.transform.position) < targetRange && ship.shipID != fighter.shipID && (target == null || ship.shipID != target.shipID)) {
                targetAngle -= Vector2.SignedAngle(ship.transform.position, transform.position) / Mathf.Clamp(1f, (ship.transform.position - transform.position).sqrMagnitude, targetRange);
            }
        }

        fighter.transform.eulerAngles = new Vector3(0, 0, targetAngle);
    }
}
