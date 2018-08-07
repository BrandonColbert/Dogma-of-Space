using UnityEngine;

public class ShipModuleTurbo : ShipModule {
    public AudioClip activationSound;

    public Color turboTrailColor = Color.blue;
    public float accelerationModifier = 5f, speedModifier = 2f;
    public float drainRate = 8f;
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

        if(charge > 0f) {
            ship.attributes.acceleration *= accelerationModifier;
            ship.attributes.speed *= speedModifier;
            ship.engineVolumeModifier *= accelerationModifier;

            AudioManager.Play(activationSound, ship.transform.position, 0.6f);
        }
    }

    public override void OnDeactivate(Ship ship) {
        base.OnDeactivate(ship);
        
        ship.attributes.acceleration = originalAcceleration;
        ship.attributes.speed = originalSpeed;
        ship.engineVolumeModifier = 1f;

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

        if(charge > 0f) {
            charge -= drainRate * Time.deltaTime;
        } else {
            charge = 0f;
            OnDeactivate(ship);
        }

        lastUseTime = Time.time;
    }

    public override void DuringNoUse(Ship ship) {
        base.DuringNoUse(ship);
        
        if(charge < 0f) {
            charge = max;
        }

        if(charge < max && Time.time - lastUseTime > 5f) {
            charge += Time.deltaTime * 10f;
            if(charge > max) charge = max;
        }
    }

    public override ModuleType GetModuleType() {
        return ModuleType.HELD;
    }

    public override string GetModuleName() {
        return "Turbo";
    }
}