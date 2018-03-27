using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : Ship {
    public float fireForce = 0.05f;
    public float fireRate = 0.2f;
    public Vector3 fireLocation;
    public GameObject primaryShell, secondaryShell;

    private float timeToFire;

    public void FireShell(bool primary) {
        if(Time.time > timeToFire) {
            GameObject shellSource = primary ? primaryShell : secondaryShell;

            if(shellSource.GetComponent<Shell>() != null && shellSource.GetComponent<Rigidbody2D>()) {
                GameObject shellObject = Instantiate(primaryShell, gameObject.transform.position + fireLocation, Quaternion.identity);
                Rigidbody2D shellRB = shellObject.GetComponent<Rigidbody2D>();
                shellRB.AddForce(new Vector2(fireForce * Mathf.Cos(gameObject.transform.rotation.eulerAngles.y), 0));
            }

            timeToFire = Time.time + fireRate;
        }
    }
}