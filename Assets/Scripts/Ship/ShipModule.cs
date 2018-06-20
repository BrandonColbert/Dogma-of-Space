using UnityEngine;

public abstract class ShipModule : MonoBehaviour {
    public enum ModuleType {
        ACTIVE,
        TOGGLE,
        HELD
    }

    public bool isActive;

    public virtual void OnActivate(Ship ship) {}
    public virtual void OnDeactivate(Ship ship) {}
    public virtual void DuringUse(Ship ship) {}
    public virtual void DuringNoUse(Ship ship) {}

    public abstract ModuleType GetModuleType();
    public abstract string GetModuleName();
}