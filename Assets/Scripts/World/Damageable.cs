using System;
using UnityEngine;

public class Damageable : MonoBehaviour {
    private Func<Damageable, bool> killCallback;
    public float maxHealth = 100f, health = 0f;

    [HideInInspector]
    public bool isDead = false;

    public Vector2 lastVelocity {
        get {
            return _lastVelocity;
        }
    }

    private Vector2 _lastVelocity;
    protected Rigidbody2D __rb__;

    public virtual void Damage(float value) {
        health -= value;
        if(health < 0) Kill();
    }

    public virtual void Kill() {
        isDead = true;

        if(!(killCallback != null && killCallback(this))) {
            Destroy(gameObject);
        }
    }
    
    ///<summary> Return true on callback to cancel the default kill function</summary>
    public void OnKill(Func<Damageable, bool> callback) {
        killCallback = callback;
    }

    public bool HasKillCallback() {
        return killCallback != null;
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