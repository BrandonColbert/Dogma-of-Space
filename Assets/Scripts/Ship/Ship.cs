using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(ShipAttributes))]
public class Ship : MonoBehaviour {
    public static List<Ship> ships = new List<Ship>();

    public ShipStatusBar statusBar;
    public ParticleSystem trail;
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

    public virtual void Spawn() {
        shipID = GetInstanceID();
        attributes = GetComponent<ShipAttributes>();
        attributes.health = attributes.maxHealth;
        attributes.shields = attributes.maxShields;
        shieldCooldown = new TimerTask();
        ships.Add(this);
	}

    public virtual void Kill() {
        ships.Remove(this);
        Destroy(gameObject);
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
            if(attributes.health < 0) Kill();
            if(statusBar != null) statusBar.SetHealth(attributes.health, attributes.maxHealth);
        }
    }

    public void Move(float forward, float rotate) {
        Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();

        rb.angularVelocity = MathHelper.ConstrainedChange(rb.angularVelocity, (rotate == 0f ? -Mathf.Sign(rb.angularVelocity) : rotate) * attributes.handling * 30f * Time.deltaTime, rotate * 270f);
        rb.velocity = MathHelper.ConstrainedVectorChange(rb.velocity, transform.up * forward * attributes.acceleration * Time.deltaTime, transform.up * forward * attributes.speed);

        if(trail) {
            if(forward == 0f) {
                if(trail.isPlaying) {
                    trail.Stop();
                }
            } else {
                if(!trail.isPlaying) {
                    trail.Play();
                }
            }
        }
    }

    public void Move(float forward) {
        Move(forward, 0f);
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
