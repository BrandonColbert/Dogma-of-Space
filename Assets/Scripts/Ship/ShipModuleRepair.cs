using System;
using System.Collections;
using UnityEngine;

public class ShipModuleRepair : ShipModule {
    public float repairCost = 1.5f;

    public override void OnActivate(Ship ship) {
        base.OnActivate(ship);
    }
    public override void OnDeactivate(Ship ship) {
        base.OnDeactivate(ship);
    }

    public override void DuringUse(Ship ship) {
        base.DuringUse(ship);

        if(charge > 0f) {
            if(ship.attributes.health < ship.attributes.maxHealth) {
                charge -= repairCost;
                ship.attributes.health++;
                if(ship.statusBar) ship.statusBar.SetHealth(ship.attributes.health, ship.attributes.maxHealth);
            } else if(ship.attributes.shields < ship.attributes.maxShields) {
                charge -= repairCost;
                ship.attributes.shields++;
                if(ship.statusBar) ship.statusBar.SetShields(ship.attributes.shields, ship.attributes.maxShields);
            }

            if(charge < 0f) {
                charge = 0f;
                isActive = false;
            }
        }
    }

    public override void DuringNoUse(Ship ship) {
        base.DuringNoUse(ship);

        if(charge < 0f) {
            charge = max;
        }

        if(charge < max) {
            if((charge += Time.deltaTime * 1f) > max) {
                charge = max;
            }
        }
    }

    public override ModuleType GetModuleType() {
        return ModuleType.TOGGLE;
    }

    public override string GetModuleName() {
        return "Repair";
    }
}