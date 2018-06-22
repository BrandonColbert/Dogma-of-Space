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
        attributes.OnKill(Kill);
        if(trail) trail.Stop();
        ships.Add(this);
	}

    public void Kill(Damageable d) {
        ships.Remove(this);
    }

    public void Move(float forward, float rotate) {
        Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();

        rb.angularVelocity = MathHelper.ConstrainedChange(rb.angularVelocity, (rotate == 0f ? -Mathf.Sign(rb.angularVelocity) : rotate) * attributes.handling * 30f * Time.deltaTime, rotate * 270f);
        rb.velocity = MathHelper.ConstrainedVectorChange(rb.velocity, transform.up * forward * attributes.acceleration * Time.deltaTime, transform.up * forward * attributes.speed);

        if(trail) {
            if(forward < 0f) {
                trail.transform.localEulerAngles = Vector3.back * 180f;
            } else {
                trail.transform.localEulerAngles = Vector3.zero;
            }

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
    }
}
