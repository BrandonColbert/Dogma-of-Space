using UnityEngine;
using UnityEngine.UI;

public class ShipStatSpecifier : MonoBehaviour {
    public Text health, shields, repairRate, repairCD, weapons, DPS, module, speed, acceleration, handling, mass;

    void Set(Text t, object v) {
        if(t && v != null) t.text = t.gameObject.name + ": " + v;
    }

    float CalculateDPS(Ship ship) {
        float d = 0;

        if(ship is Fighter) {
            Fighter fighter = ship as Fighter;

            foreach(FighterWeapon weapon in fighter.weapons) {
                d += weapon.fireDamage * weapon.fireRate;
            }

            if(fighter.alternatingWeapons) {
                d /= 2f;
            }
        }

        return d;
    }

    public void Display(Ship ship) {
        ShipAttributes attributes = ship.GetComponent<ShipAttributes>();

        Set(health, attributes.maxHealth);
        Set(shields, attributes.maxShields);
        Set(repairRate, attributes.shieldRepairRate + "p");
        Set(repairCD, attributes.shieldRepairCooldown + "s");
        Set(weapons, (ship is Fighter) ? (ship as Fighter).weapons.Length : 0);
        Set(DPS, CalculateDPS(ship));
        Set(module, ship.module ? ship.module.GetModuleName() : "None");
        Set(speed, attributes.speed * 10f);
        Set(acceleration, attributes.acceleration * 10f);
        Set(handling, attributes.handling);
        Set(mass, (ship.GetComponent<Rigidbody2D>() ? ship.GetComponent<Rigidbody2D>().mass : -1f) * 10 + "kg");
    }
}