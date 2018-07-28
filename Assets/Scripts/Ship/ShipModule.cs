using System;
using UnityEngine;

public abstract class ShipModule : MonoBehaviour {
    public enum ModuleType {
        ACTIVE,
        TOGGLE,
        HELD
    }

    private Ship ship;

    public bool isActive;

    private float currentCharge = -1f;
    private float maxCharge = 100f;
    
    protected float charge {
        set {
            currentCharge = value;
            if(ship.statusBar) ship.statusBar.SetSpecial(currentCharge, maxCharge);
        }
        get {
            return currentCharge;
        }
    }

    protected float max {
        get {
            return maxCharge;
        }
    }

    public virtual void OnActivate(Ship ship) { this.ship = ship; }
    public virtual void OnDeactivate(Ship ship) { this.ship = ship; }
    public virtual void DuringUse(Ship ship) { this.ship = ship; }
    public virtual void DuringNoUse(Ship ship) { this.ship = ship; }
    public virtual void OnWeaponFire(FighterWeapon weapon) { this.ship = weapon.ship; }
    public abstract ModuleType GetModuleType();
    public abstract string GetModuleName();
    public virtual bool GetDefaultState() { return false; }
}