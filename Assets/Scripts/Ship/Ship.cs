using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(ShipAttributes))]
public class Ship : MonoBehaviour {
    public static List<Ship> ships = new List<Ship>();

    public ShipStatusBar statusBar;
    public ShipController controller;
    public ShipModule module;
    public TimerTask shieldCooldown;

    [HideInInspector]
    public int shipID;
    [HideInInspector]
    public ShipAttributes attributes;

    void Start() {
        Spawn();
    }

    void Awake() {
        Spawn();
    }

    public virtual void Spawn() {
        shipID = GetInstanceID();
        attributes = GetComponent<ShipAttributes>();
        attributes.health = attributes.maxHealth;
        attributes.shields = attributes.maxShields;
        shieldCooldown = new TimerTask();
        ships.Add(this);
	}

    public virtual void Damage(float value) {
        if(attributes.shields > 0) {
            attributes.shields -= value;
            value = attributes.shields < 0 ? -attributes.shields : 0;
            shieldCooldown.Set(Time.time);
            if(statusBar != null) statusBar.SetShields(attributes.shields, attributes.maxShields);
        }

        if(value > 0) {
            attributes.health -= value;
            if(attributes.health < 0) Destroy(gameObject);
            if(statusBar != null) statusBar.SetHealth(attributes.health, attributes.maxHealth);
        }
    }

    protected Vector2 ConstrainedVectorChange(Vector2 value, Vector2 change, Vector2 target) {
        return new Vector2(ConstrainedChange(value.x, change.x, target.x), ConstrainedChange(value.y, change.y, target.y));
    }

    protected float ConstrainedChange(float value, float change, float target) {
        float n = value + change;
        float r = value;
        
        if(value < target) {
            if(n > target) {
                r = target;
            } else if(n > value) {
                r = n;
            }
        } else if(value > target) {
            if(n < target) {
                r = target;
            } else if(n < value) {
                r = n;
            }
        }

        return r;
    }

    public void Move(float forward, float rotate) {
        Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();

        rb.angularVelocity = ConstrainedChange(rb.angularVelocity, (rotate == 0f ? -Mathf.Sign(rb.angularVelocity) : rotate) * attributes.handling * 30f * Time.deltaTime, rotate * 270f);
        rb.velocity = ConstrainedVectorChange(rb.velocity, transform.up * forward * attributes.acceleration * Time.deltaTime, transform.up * forward * attributes.speed);
    }

    public virtual void FixedUpdate() {
        if(controller != null) controller.PhysicsLogic(this);
    }

    public virtual void Update() {
        if(controller != null) controller.Logic(this);

        if(attributes.shields < attributes.maxShields && shieldCooldown.Interval(attributes.shieldRepairCooldown).Ready()) {
            attributes.shields += attributes.shieldRepairRate * Time.deltaTime;
            if(attributes.shields > attributes.maxShields) attributes.shields = attributes.maxShields;
            if(statusBar != null) statusBar.SetShields(attributes.shields, attributes.maxShields);
        }
    }
}
