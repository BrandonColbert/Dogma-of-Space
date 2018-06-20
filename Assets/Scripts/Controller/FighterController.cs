using UnityEngine;

[RequireComponent(typeof(Fighter))]
public abstract class FighterController : ShipController {
    public override void Logic(Ship ship) {
        Logic((Fighter)ship);
    }

    public abstract void Logic(Fighter fighter);

    public override void PhysicsLogic(Ship ship) {
        PhysicsLogic((Fighter)ship);
    }

    public abstract void PhysicsLogic(Fighter fighter);
}