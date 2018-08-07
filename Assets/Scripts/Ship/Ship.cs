using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(ShipAttributes))]
public class Ship : MonoBehaviour {
    public static List<Ship> ships = new List<Ship>();

    public ShipStatusBar statusBar;
    [Range(0f, 1f)] public float engineVolume = 0.1f;
    public float engineVolumeModifier = 1f;
    [Range(0f, 5f)] public float engineSoundLoopStart = 0f, engineSoundLoopEnd = 1f;
    public AudioClip damageSound, shieldDamageSound;
    public ParticleSystem[] trails;
    public ShipController controller;
    public ShipModule module;

    [HideInInspector]
    public int shipID;
    [HideInInspector] public ShipAttributes attributes;
    
    private Rigidbody2D rb;
    [HideInInspector] public AudioSource engineSound;

    public virtual void Start() {
        shipID = GetInstanceID();
        rb = gameObject.GetComponent<Rigidbody2D>();
        attributes = GetComponent<ShipAttributes>();
        engineSound = GetComponent<AudioSource>();

        if(!attributes.HasKillCallback()) {
            attributes.OnKill(RemoveWhenKilled);
        }

        if(trails.Length > 0) foreach(ParticleSystem trail in trails) {
            trail.Stop();
        }

        if(module && module.GetDefaultState()) {
            module.isActive = true;
            module.OnActivate(this);
        }

        ships.Add(this);
    }

    public void SetMinimap(GameObject minimapIconPrefab) {
        GameObject mi = Instantiate(minimapIconPrefab);
        mi.transform.parent = transform;
        mi.transform.rotation = Quaternion.identity;
        mi.transform.position = Vector3.zero;
    }

    public bool RemoveWhenKilled(Damageable d) {
        ships.Remove(this);

        return false;
    }

    public void Move(float forward, float rotate = 0f) {
        rb.angularVelocity = MathHelper.ConstrainedChange(rb.angularVelocity, (rotate == 0f ? -Mathf.Sign(rb.angularVelocity) : rotate) * attributes.handling * 30f * Time.deltaTime, rotate * 270f);
        rb.velocity = MathHelper.ConstrainedVectorChange(rb.velocity, transform.up * forward * attributes.acceleration * Time.deltaTime, transform.up * forward * attributes.speed);

        if(trails.Length > 0) {
            if(forward < 0f) {
                foreach(ParticleSystem trail in trails) trail.transform.localEulerAngles = Vector3.back * 180f;
            } else {
                foreach(ParticleSystem trail in trails) trail.transform.localEulerAngles = Vector3.zero;
            }

            if(forward == 0f) {
                if(engineSound && engineSound.isPlaying && engineSound.time < engineSoundLoopEnd) {
                    engineSound.time = engineSoundLoopEnd;
                }

                foreach(ParticleSystem trail in trails) {
                    if(trail.isPlaying) {
                        trail.Stop();
                    }
                }
            } else {
                foreach(ParticleSystem trail in trails) {
                    if(!trail.isPlaying) {
                        trail.Play();
                    }
                }

                if(engineSound) {
                    engineSound.volume = engineVolume * engineVolumeModifier * AudioManager.volumeModifier * Mathf.Clamp(Camera.main.orthographicSize / Vector2.Distance(Camera.main.gameObject.transform.position, transform.position), 0f, 1f);

                    if(engineSound.isPlaying) {
                        if(engineSound.time > engineSoundLoopEnd) {
                            engineSound.time = engineSoundLoopStart;
                        }
                    } else {
                        engineSound.Play();
                    }
                }
            }
        }
    }

    public virtual void FixedUpdate() {
        if(controller) controller.PhysicsLogic(this);
    }

    public virtual void Update() {
        if(controller) controller.Logic(this);
    }
}
