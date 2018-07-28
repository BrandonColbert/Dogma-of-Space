using System;
using System.Collections;
using UnityEngine;

public class ShipModuleWarp : ShipModule {
    public GameObject trailPrefab;
    private TrailRenderer trail;

    void Start() {
        trail = Instantiate(trailPrefab, Vector3.zero, Quaternion.identity, transform).GetComponent<TrailRenderer>();
        trail.enabled = false;
    }

    public override void OnActivate(Ship ship) {
        base.OnActivate(ship);

        if(charge > 0f && !trail.enabled) {
            StartCoroutine(Warp(ship));
        }
    }

    IEnumerator Warp(Ship ship) {
        Vector3 initial = ship.transform.position;
        Vector3 final = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        final.z = initial.z;

        float cost = Vector2.Distance(initial, final) / 2f;
        if(charge < cost) yield break;

        Vector3 angles = ship.transform.eulerAngles;
        angles.z = Mathf.Rad2Deg * Mathf.Atan2(final.y - initial.y, final.x - initial.x) - 90f;
        ship.transform.eulerAngles = angles;

        trail.enabled = true;
        Rigidbody2D rb = ship.transform.GetComponent<Rigidbody2D>();
        rb.angularVelocity = 0f;
        rb.velocity = Vector2.zero;

        float n = trail.time * 100f;

        for(int i = 0; i < n; i++) {
            float p = (float)(1 + i) / n;

            charge -= cost / n;
            ship.transform.position = new Vector2(initial.x + (final.x - initial.x) * p, initial.y + (final.y - initial.y) * p);

            yield return new WaitForSeconds(0.05f / n);
        }

        if(charge < 0f) charge = 0f;
        trail.enabled = false;
    }

    public override void OnDeactivate(Ship ship) {
        base.OnDeactivate(ship);
    }

    public override void DuringUse(Ship ship) {
        base.DuringUse(ship);
    }

    public override void DuringNoUse(Ship ship) {
        base.DuringNoUse(ship);

        if(charge < 0f) {
            charge = max;
        }

        if(charge < max && trail && !trail.enabled) {
            if((charge += Time.deltaTime * 2f) > max) {
                charge = max;
            }
        }
    }

    public override ModuleType GetModuleType() {
        return ModuleType.ACTIVE;
    }

    public override string GetModuleName() {
        return "Warp";
    }
}