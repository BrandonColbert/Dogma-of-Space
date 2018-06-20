using UnityEngine;
using System.Collections;

public class EnemyShipAI : FighterController {
    public Ship target;
    public float targetRange = 50f;
    public float minDistance = 5f;
    public float maxDistance = 10f;

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
        if(target) {
            Vector2 dif = target.transform.position - transform.position;
            float deg = Mathf.Atan2(dif.y, dif.x) * Mathf.Rad2Deg - 90f;
            fighter.transform.eulerAngles = new Vector3(0, 0, deg);
            
            if(dif.magnitude > maxDistance) {
                fighter.Move(1, 0);
            } else if(dif.magnitude < minDistance) {
                fighter.Move(-1, 0);
            } else {
                fighter.Move(0, 0);
            }

            fighter.Aim(target.transform.position);
            fighter.Fire();
        } else {
            fighter.Move(MathHelper.Rand(0, 1), MathHelper.Rand(-1f, 1f));
        }
    }
}
