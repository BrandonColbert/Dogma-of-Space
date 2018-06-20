using UnityEngine;

[RequireComponent(typeof(Ship))]
public abstract class ShipController : MonoBehaviour {
    public abstract void Logic(Ship ship);
    public abstract void PhysicsLogic(Ship ship);
}