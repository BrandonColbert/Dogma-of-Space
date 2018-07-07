using UnityEngine;

public class PlayerFighterController : FighterController {
    public Player player;

    void Start() {
        if(!player) {
            Destroy(this);
            throw new UnassignedReferenceException("Player not assigned in player fighter controller");
        }
    }

    public override void PhysicsLogic(Fighter fighter) {
        fighter.Move(Input.GetAxis("Vertical"), -Input.GetAxis("Horizontal"));

        if(fighter.module != null) {
            switch(fighter.module.GetModuleType()) {
                case ShipModule.ModuleType.ACTIVE:
                    if(Input.GetKeyDown(KeyCode.Space)) fighter.module.OnActivate(fighter);
                    break;
                case ShipModule.ModuleType.HELD:
                    if(Input.GetKey(KeyCode.Space)) {
                        if(fighter.module.isActive) {
                            fighter.module.DuringUse(fighter);
                        } else {
                            fighter.module.isActive = true;
                            fighter.module.OnActivate(fighter);
                        }
                    } else if(fighter.module.isActive) {
                        fighter.module.isActive = false;
                        fighter.module.OnDeactivate(fighter);
                    } else {
                        fighter.module.DuringNoUse(fighter);
                    }
                    break;
                case ShipModule.ModuleType.TOGGLE:
                    if(Input.GetKeyDown(KeyCode.Space)) {
                        fighter.module.isActive = !fighter.module.isActive;

                        if(fighter.module.isActive) {
                            fighter.module.OnActivate(fighter);
                        } else {
                            fighter.module.OnDeactivate(fighter);
                        }
                    }

                    if(fighter.module.isActive) {
                        fighter.module.DuringUse(fighter);
                    } else {
                        fighter.module.DuringNoUse(fighter);
                    }
                    break;
            }
        }

        fighter.Aim(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    	if(Input.GetMouseButton(0)) fighter.Fire();

        if(Input.GetKeyDown(KeyCode.K)) {
            fighter.attributes.Kill();
            if(player.spawnOnScriptStart) player.Spawn();
        }
    }

    public override void Logic(Fighter fighter) {}
}