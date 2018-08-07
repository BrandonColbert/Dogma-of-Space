using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ShieldEffect : Indestructable {
    public ShipAttributes attributes;
    public float fadeDelay = 0.25f;
    public float fadeSteps = 100f;
    public AudioClip destroySound;

    private bool areShieldsDestroyed;
    private SpriteRenderer srender;
    
    public Color color {
        get {
            return srender.color;
        }

        set {
            srender.color = value;
        }
    }

    private CircleCollider2D shieldCollider;

    public override void Start() {
        base.Start();
        __rb__ = attributes.GetComponent<Rigidbody2D>();

        srender = GetComponent<SpriteRenderer>();
        shieldCollider = gameObject.AddComponent<CircleCollider2D>();
        
        ChangeAlpha(0f);
        shieldCollider.enabled = false;

        areShieldsDestroyed = true;
    }

    void ChangeAlpha(float a) {
        color = new Color(color.r, color.g, color.b, a);
    }

    IEnumerator ShieldFlash() {
        ChangeAlpha(1f);
        yield return null;

        shieldCollider.enabled = false;
        float j = 1f / fadeSteps;

        for(float i = 1f; i >= 0; i -= j) {
            ChangeAlpha(i);
            yield return null;
        }

        ChangeAlpha(0f);
    }

    public override void Damage(float value) {
        attributes.Damage(value);
    }

    void Update() {
        if(attributes.shields <= 0f) {
            if(!areShieldsDestroyed) {
                AudioManager.Play(destroySound, transform.position, 2f);
            }

            areShieldsDestroyed = true;

            StopCoroutine("ShieldFlash");
            ChangeAlpha(0f);
            shieldCollider.enabled = false;
        } else {
            areShieldsDestroyed = false;

            if(attributes.shields != attributes.maxShields && (Time.time - attributes.shieldCooldown.Last()) <= fadeDelay) {
                StopCoroutine("ShieldFlash");
                shieldCollider.enabled = true;
                StartCoroutine("ShieldFlash");
            }
        }
    }
}