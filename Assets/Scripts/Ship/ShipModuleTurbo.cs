using UnityEngine;

public class ShipModuleTurbo : ShipModule {
    public Color turboTrailColor = Color.blue;
    public float accelerationModifier = 5f, speedModifier = 2f;
    public float fuelCapacity = 100f, fuel = -1;
    private float lastUseTime;
    private float originalAcceleration, originalSpeed;
    private UnityEngine.ParticleSystem.MinMaxGradient originalTrailColor;
    private UnityEngine.ParticleSystem.MinMaxCurve originalEmissionRoT;
    private UnityEngine.ParticleSystem.MinMaxCurve originalStartSpeed;

    public override void OnActivate(Ship ship) {
        base.OnActivate(ship);
        
        originalAcceleration = ship.attributes.acceleration;
        originalSpeed = ship.attributes.speed;

        foreach(ParticleSystem trail in ship.trails) {
            if(trail) {
                originalTrailColor = trail.main.startColor;
                originalEmissionRoT = trail.emission.rateOverTime;
                originalStartSpeed = trail.main.startSpeed;

                ParticleSystem.MainModule psmm = trail.main;
                psmm.startColor = turboTrailColor;
                psmm.startSpeed = originalStartSpeed.constant * speedModifier;

                ParticleSystem.EmissionModule psem = trail.emission;
                psem.rateOverTime = originalEmissionRoT.constant * 5f;
            }
        }

        if(fuel > 0f) {
            ship.attributes.acceleration *= accelerationModifier;
            ship.attributes.speed *= speedModifier;
        }
    }

    public override void OnDeactivate(Ship ship) {
        base.OnDeactivate(ship);
        
        ship.attributes.acceleration = originalAcceleration;
        ship.attributes.speed = originalSpeed;

        foreach(ParticleSystem trail in ship.trails) {
            if(trail) {
                ParticleSystem.MainModule psmm = trail.main;
                psmm.startColor = originalTrailColor;
                psmm.startSpeed = originalStartSpeed;

                ParticleSystem.EmissionModule psem = trail.emission;
                psem.rateOverTime = originalEmissionRoT;
            }
        }
    }

    public override void DuringUse(Ship ship) {
        base.DuringUse(ship);

        if(fuel > 0f) {
            fuel -= 20f * Time.deltaTime;
            if(ship.statusBar) ship.statusBar.SetSpecial(fuel, fuelCapacity);
        } else {
            fuel = 0f;
            OnDeactivate(ship);
        }

        lastUseTime = Time.time;
    }

    public override void DuringNoUse(Ship ship) {
        if(fuel < 0f) {
            fuel = fuelCapacity;
            if(ship.statusBar) ship.statusBar.SetSpecial(fuel, fuelCapacity);
        }

        if(fuel < fuelCapacity && Time.time - lastUseTime > 5f) {
            fuel += Time.deltaTime * 10f;
            if(fuel > fuelCapacity) fuel = fuelCapacity;

            if(ship.statusBar) ship.statusBar.SetSpecial(fuel, fuelCapacity);
        }
    }

    public override ModuleType GetModuleType() {
        return ModuleType.HELD;
    }

    public override string GetModuleName() {
        return "Turbo";
    }
}