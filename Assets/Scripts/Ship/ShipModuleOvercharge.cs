using System;
using System.Collections;
using UnityEngine;

public class ShipModuleOvercharge : ShipModule {
    public float fireRateMultiplier = 10f;
    public float depleteRate = 5f;
    public float rechargeRate = 2f;
    public float waitInterval = 0.25f;

    private float lastUseTime;
    private float[] firerates = null;
    private bool lastOverchargeState = false;

    void Start() {
        lastUseTime = Time.time;
    }

    public override void OnActivate(Ship ship) {
        base.OnActivate(ship);
    }
    public override void OnDeactivate(Ship ship) {
        base.OnDeactivate(ship);
        ToggleOvercharge(ship, false);
    }

    public override void DuringUse(Ship ship) {
        base.DuringUse(ship);
        Recharge(ship);

        if(charge <= 0f) {
            ToggleOvercharge(ship, false);
        } else {
            ToggleOvercharge(ship, true);
        }
    }

    public override void DuringNoUse(Ship ship) {
        base.DuringNoUse(ship);
        Recharge(ship);
    }

    public void ToggleOvercharge(Ship ship, bool state) {
        if(state != lastOverchargeState) {
            if(ship is Fighter) {
                FighterWeapon[] weapons = (ship as Fighter).weapons;

                if(firerates == null) {
                    firerates = new float[weapons.Length];

                    for(int i = 0; i < weapons.Length; i++) {
                        firerates[i] = weapons[i].fireRate;
                    }
                }

                for(int i = 0; i < weapons.Length; i++) {
                    weapons[i].fireRate = firerates[i] * (state ? fireRateMultiplier : 1f);
                }
            }

            lastOverchargeState = state;
        }
    }

    public void Recharge(Ship ship) {
        if(charge < 0f) {
            charge = max;
        }

        if(charge < max && Time.time - lastUseTime > waitInterval) {
            if((charge += Time.deltaTime * rechargeRate) > max) {
                charge = max;
            }
        }
    }

    public override void OnWeaponFire(FighterWeapon weapon) {
        base.OnWeaponFire(weapon);

        if(isActive) {
            if((charge -= depleteRate) < 0f) charge = 0f;
            lastUseTime = Time.time;
        }
    }

    public override ModuleType GetModuleType() {
        return ModuleType.TOGGLE;
    }

    public override string GetModuleName() {
        return "Overcharge";
    }

    public override bool GetDefaultState() {
        return true;
    }
}