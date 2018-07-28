using System;
using System.Collections;
using UnityEngine;

public class ShipModuleFireMode : ShipModule {
    public override void OnActivate(Ship ship) {
        base.OnActivate(ship);
        if(ship is Fighter) ((Fighter)ship).alternatingWeapons = true;
    }
    public override void OnDeactivate(Ship ship) {
        base.OnDeactivate(ship);
        if(ship is Fighter) ((Fighter)ship).alternatingWeapons = false;
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
}