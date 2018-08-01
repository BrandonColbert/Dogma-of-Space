using UnityEngine;

public class PlayerFighterController : FighterController {
    public Player player;
    [HideInInspector] public Color? specialColor = null;

    void Start() {
        if(!player) {
            Destroy(this);
            throw new UnassignedReferenceException("Player not assigned in player fighter controller");
        }
    }

    public override void PhysicsLogic(Fighter fighter) {
        if(!DosMenu.isMenuOpen) {
            fighter.Move(Input.GetAxis("Vertical"), -Input.GetAxis("Horizontal"));

            if(fighter.module != null) {
                switch(fighter.module.GetModuleType()) {
                    case ShipModule.ModuleType.ACTIVE:
                        if(Input.GetKeyDown(KeyCode.Space)) fighter.module.OnActivate(fighter);
                        else fighter.module.DuringNoUse(fighter);
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
                            if(fighter.module.isActive) {
                                fighter.module.isActive = false;
                                fighter.module.OnDeactivate(fighter);
                            } else {
                                fighter.module.isActive = true;
                                fighter.module.OnActivate(fighter);
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
        }
    }

    public override void Logic(Fighter fighter) {
        if(fighter.module) fighter.statusBar.specialTexture.color = specialColor.Value * (fighter.module.isActive ? 2.4f : 1f);
    }
}