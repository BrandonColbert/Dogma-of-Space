﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Fighter : Ship {
    public bool alternatingWeapons;
    public FighterWeapon[] weapons;

    private int currentWeapon, lastWeapon;

    public override void Start() {
        base.Start();

        lastWeapon = currentWeapon = 0;

        foreach(FighterWeapon weapon in weapons) {
            weapon.ship = this;
        }
    }

    public void Aim(Vector2 direction) {
        foreach(FighterWeapon weapon in weapons) {
            weapon.fireDirection = direction;
        }
    }

    public virtual void Fire() {
        if(alternatingWeapons) {
            if(currentWeapon >= weapons.Length) {
                currentWeapon = 0;
            }

            if(weapons[lastWeapon].timer.Ready() && weapons[currentWeapon].Fire()) {
                lastWeapon = currentWeapon;
                currentWeapon++;
            }
        } else {
            foreach(FighterWeapon weapon in weapons) {
                weapon.Fire();
                if(module) module.OnWeaponFire(weapon);
            }
        }
    }

    public bool ReadyToFire() {
        if(alternatingWeapons) {
            return weapons[lastWeapon].timer.Ready() && weapons[currentWeapon].Fire();
        } else {
            return true;
        }
    }

    public bool CanBeAimed() {
        return weapons.Any(w => w.aimable);
    }
}