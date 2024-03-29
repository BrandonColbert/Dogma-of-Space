using UnityEngine;

[RequireComponent(typeof(Ship))]
public class ShipAttributes : Damageable {
    public float maxShields = 100f, shields, shieldRepairRate = 5f, shieldRepairCooldown = 10f;
    public float speed = 10f, acceleration = 5f, handling = 100f;

    [HideInInspector]
    public TimerTask shieldCooldown;
    private Ship ship;

    public override void Start() {
        base.Start();
        shieldCooldown = new TimerTask();
        ship = GetComponent<Ship>();

        health = maxHealth;
        shields = maxShields;

        if(ship.statusBar && ship.statusBar.isActiveAndEnabled) ship.statusBar.SetHealth(health, maxHealth);
        if(ship.statusBar && ship.statusBar.isActiveAndEnabled) ship.statusBar.SetShields(shields, maxShields);
    }

    public override void Damage(float value) {
        if(shields > 0) {
            shields -= value;
            value = shields < 0 ? -shields : 0;
            shieldCooldown.Set(Time.time);
            if(ship.statusBar && ship.statusBar.isActiveAndEnabled) ship.statusBar.SetShields(shields, maxShields);
            if(ship.shieldDamageSound) AudioManager.Play(ship.shieldDamageSound, ship.transform.position, UnityEngine.Random.Range(1f, 1.75f));
        } else {
            if(ship.damageSound) AudioManager.Play(ship.damageSound, ship.transform.position, UnityEngine.Random.Range(1f, 1.75f));
        }

        if(value > 0) {
            base.Damage(value);
            if(ship.statusBar && ship.statusBar.isActiveAndEnabled) ship.statusBar.SetHealth(health, maxHealth);
        }
    }

    public virtual void Update() {
        if(shields < maxShields && shieldCooldown.Interval(shieldRepairCooldown).Ready()) {
            shields += shieldRepairRate * Time.deltaTime;
            if(shields > maxShields) shields = maxShields;
            if(ship.statusBar && ship.statusBar.isActiveAndEnabled) ship.statusBar.SetShields(shields, maxShields);
        }
    }
}