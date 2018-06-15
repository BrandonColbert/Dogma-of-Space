using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(ShipAttributes))]
public class Ship : MonoBehaviour {
    public ShipModule module;

    [HideInInspector]
    public int shipID;
    [HideInInspector]
    public ShipAttributes attributes;
    [HideInInspector]
    public float currentSpeed, lastForwards;

    void Start() {
        Spawn();
    }

    public virtual void Spawn() {
        shipID = GetInstanceID();
        attributes = GetComponent<ShipAttributes>();
        attributes.health = attributes.maxHealth;
        attributes.shields = attributes.maxShields;
	}

    public void Damage(float value) {
        if(attributes.shields > 0) {
            attributes.shields -= value;
            value = attributes.shields < 0 ? -attributes.shields : 0;
        }

        attributes.health -= value;

        if(attributes.health < 0) {
            Destroy(gameObject);
        }
    }

    void Move(float forward, float rotate) {
        Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
        
        transform.Rotate(Vector3.forward, rotate * attributes.handling * Time.deltaTime);

        if(Mathf.Abs(forward) >= 1f || (forward < 0 ? forward < lastForwards : forward > lastForwards)) {
            currentSpeed = currentSpeed < attributes.speed ? currentSpeed + attributes.acceleration * Time.deltaTime : attributes.speed;
            rb.velocity = transform.up * forward * currentSpeed * Time.deltaTime;
            rb.angularVelocity = 0;
        } else {
            if(currentSpeed > 1f) {
                rb.velocity = rb.velocity.normalized * currentSpeed * Time.deltaTime;
                currentSpeed *= 0.995f;
            } else {
                currentSpeed = 0f;
            }
        }

        lastForwards = forward;
    }

    public virtual void FixedUpdate() {
        Move(Input.GetAxis("Vertical"), -Input.GetAxis("Horizontal"));

        if(module != null) {
            switch(module.GetModuleType()) {
                case ShipModule.ModuleType.ACTIVE:
                    if(Input.GetKeyDown(KeyCode.Space))  module.OnActivate(this);
                    break;
                case ShipModule.ModuleType.HELD:
                    if(Input.GetKey(KeyCode.Space)) module.DuringUse(this);
                    break;
                case ShipModule.ModuleType.TOGGLE:
                    if(Input.GetKeyDown(KeyCode.Space)) module.OnActivate(this);
                    else if(Input.GetKeyUp(KeyCode.Space)) module.OnDeactivate(this);
                    else if(Input.GetKey(KeyCode.Space)) module.DuringUse(this);
                    break;
            }
        }
    }
}
