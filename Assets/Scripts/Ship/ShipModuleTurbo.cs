using UnityEngine;

public class ShipModuleTurbo : ShipModule {
    public float speedModifier = 2f;
    private float originalSpeed;

    public override void OnActivate(Ship ship) {
        originalSpeed = ship.attributes.speed;
        ship.attributes.speed = ship.attributes.speed * speedModifier;
    }

    public override void OnDeactivate(Ship ship) {
        ship.attributes.speed = originalSpeed;
    }

    public override void DuringUse(Ship ship) {
        ship.currentSpeed = ship.attributes.speed;
    }

    public override ModuleType GetModuleType() {
        return ModuleType.TOGGLE;
    }

    public override string GetModuleName() {
        return "Turbo";
    }
}