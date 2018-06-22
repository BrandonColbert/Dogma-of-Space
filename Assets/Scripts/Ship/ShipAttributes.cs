using UnityEngine;

[RequireComponent(typeof(Ship))]
public class ShipAttributes : Damageable {
    public float maxShields = 100f, shields, shieldRepairRate = 5f, shieldRepairCooldown = 10f;
    public float speed = 10f, acceleration = 5f, handling = 100f;

    private TimerTask shieldCooldown;
    private Ship ship;

    public override void Start() {
        base.Start();
        shieldCooldown = new TimerTask();
        ship = GetComponent<Ship>();
    }

    public override void Damage(float value) {
        if(shields > 0) {
            shields -= value;
            value = shields < 0 ? -shields : 0;
            shieldCooldown.Set(Time.time);
            if(ship.statusBar) ship.statusBar.SetShields(shields, maxShields);
        }

        if(value > 0) {
            base.Damage(value);
            if(ship.statusBar) ship.statusBar.SetHealth(health, maxHealth);
        }
    }

    public virtual void Update() {
        if(shields < maxShields && shieldCooldown.Interval(shieldRepairCooldown).Ready()) {
            shields += shieldRepairRate * Time.deltaTime;
            if(shields > maxShields) shields = maxShields;
            if(ship.statusBar) ship.statusBar.SetShields(shields, maxShields);
        }
    }
}