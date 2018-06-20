using UnityEngine;

public class ShipModuleTurbo : ShipModule {
    public float accelerationModifier = 5f, speedModifier = 2f;
    public float fuelCapacity = 100f, fuel = -1;
    private float lastUseTime;
    private float originalAcceleration, originalSpeed;

    public override void OnActivate(Ship ship) {
        base.OnActivate(ship);
        
        originalAcceleration = ship.attributes.acceleration;
        originalSpeed = ship.attributes.speed;

        if(fuel > 0f) {
            ship.attributes.acceleration *= accelerationModifier;
            ship.attributes.speed *= speedModifier;
        }
    }

    public override void OnDeactivate(Ship ship) {
        base.OnDeactivate(ship);
        
        ship.attributes.acceleration = originalAcceleration;
        ship.attributes.speed = originalSpeed;
    }

    public override void DuringUse(Ship ship) {
        base.DuringUse(ship);

        if(fuel > 0f) {
            fuel -= 20f * Time.deltaTime;
            ship.statusBar.SetSpecial(fuel, fuelCapacity);
        } else {
            fuel = 0f;
            OnDeactivate(ship);
        }

        lastUseTime = Time.time;
    }

    public override void DuringNoUse(Ship ship) {
        if(fuel < 0f) {
            fuel = fuelCapacity;
            ship.statusBar.SetSpecial(fuel, fuelCapacity);
        }

        if(fuel < fuelCapacity && Time.time - lastUseTime > 5f) {
            fuel += Time.deltaTime * 10f;
            if(fuel > fuelCapacity) fuel = fuelCapacity;

            ship.statusBar.SetSpecial(fuel, fuelCapacity);
        }
    }

    public override ModuleType GetModuleType() {
        return ModuleType.HELD;
    }

    public override string GetModuleName() {
        return "Turbo";
    }
}