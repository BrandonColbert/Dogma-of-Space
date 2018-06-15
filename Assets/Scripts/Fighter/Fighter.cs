using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : Ship {
    public FighterWeapon[] weapons;

    public override void Spawn() {
        base.Spawn();

        foreach(FighterWeapon weapon in weapons) {
            weapon.ship = this;
        }
    }

    public override void FixedUpdate() {
    	base.FixedUpdate();

    	if(Input.GetMouseButton(0)) {
            foreach(FighterWeapon weapon in weapons) {
                weapon.Fire();
            }
        }
    }
}