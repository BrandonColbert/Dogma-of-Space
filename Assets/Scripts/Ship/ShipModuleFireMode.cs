using System;
using System.Collections;
using UnityEngine;

public class ShipModuleFireMode : ShipModule {
    private float[] damages = null;
    private float[] firerates = null;

    public override void OnActivate(Ship ship) {
        base.OnActivate(ship);
        ToggleAlternating(ship, false);
    }
    public override void OnDeactivate(Ship ship) {
        base.OnDeactivate(ship);
        ToggleAlternating(ship, true);
    }

    public override void DuringUse(Ship ship) {
        base.DuringUse(ship);
    }

    public override void DuringNoUse(Ship ship) {
        base.DuringNoUse(ship);
    }

    public override ModuleType GetModuleType() {
        return ModuleType.TOGGLE;
    }

    public override string GetModuleName() {
        return "Fire Mode";
    }

    public void ToggleAlternating(Ship ship, bool state) {
        if(ship is Fighter) {
            FighterWeapon[] weapons = (ship as Fighter).weapons;

            if(firerates == null) {
                firerates = new float[weapons.Length];

                for(int i = 0; i < weapons.Length; i++) {
                    firerates[i] = weapons[i].fireRate;
                }
            }

            if(damages == null) {
                damages = new float[weapons.Length];

                for(int i = 0; i < weapons.Length; i++) {
                    damages[i] = weapons[i].fireDamage;
                }
            }

            for(int i = 0; i < weapons.Length; i++) {
                weapons[i].fireRate = firerates[i] * (state ? 1f : 0.5f);
                weapons[i].fireDamage = damages[i] * (state ? 1f : 1.5f);
            }

           (ship as Fighter).alternatingWeapons = state;
        }
    }
}