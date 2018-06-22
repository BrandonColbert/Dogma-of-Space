using System;
using UnityEngine;

public class Damageable : MonoBehaviour {
    private Action<Damageable> killCallback;
    public float maxHealth = 100f, health = 0f;

    [HideInInspector]
    public bool isDead = false;

    public Vector2 lastVelocity {
        get {
            return _lastVelocity;
        }
    }

    private Vector2 _lastVelocity;
    private Rigidbody2D __rb__;

    public virtual void Damage(float value) {
        health -= value;
        if(health < 0) Kill();
    }

    public virtual void Kill() {
        isDead = true;
        if(killCallback != null) killCallback(this);
        Destroy(gameObject);
    }
    
    public void OnKill(Action<Damageable> callback) {
        killCallback = callback;
    }

    public virtual void Start() {
        __rb__ = GetComponent<Rigidbody2D>();
    }

    public virtual void FixedUpdate() {
        if(__rb__) {
            _lastVelocity = __rb__.velocity;
        }
    }
}