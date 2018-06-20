using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour {
    public GameObject asteroidPrefab;
    public float width = 100f, height = 100f;
    public int minAsteroidCount = 10, maxAsteroidCount = 100;
    public int asteroidMinSides = 5, asteroidMaxSides = 15;
    public float asteroidMinRadius = 0.5f, asteroidMaxRadius = 5f;
    public float minimumAsteroidSeperation = 12f;
    public float asteroidDensity = 5f;
    public float healthToMassRatio = 1f;

    public float minStartingSpeed = 0f, maxStartingSpeed = 250f;
    public bool pushOutToFit = false;

    void Start() {
        SpawnAsteroids();
    }

    void SpawnAsteroids() {
        int amount = MathHelper.Rand(minAsteroidCount, maxAsteroidCount);
        Vector2[] spawnpoints = new Vector2[amount];
        bool relapse = true;

        for(int i = 0; i < amount; i++) {
            spawnpoints[i] = new Vector2(MathHelper.Rand(-width / 2f, width / 2f), MathHelper.Rand(-height / 2f, height / 2f));
        }

        while(relapse) {
            relapse = false;

            for(int i = 0; i < spawnpoints.Length; i++) {
                for(int j = 0; j < spawnpoints.Length; j++) {
                    if(i != j) {
                        Vector2 a = spawnpoints[i];
                        Vector2 b = spawnpoints[j];

                        while(Vector2.Distance(a, b) < minimumAsteroidSeperation) {
                            relapse = true;

                            Debug.Log("(" + (i * spawnpoints.Length + j) + "/" + (spawnpoints.Length * spawnpoints.Length) + ") Moving " + a + " away from " + b);

                            a += ((a - b).normalized + (pushOutToFit ? (a - (Vector2)transform.position).normalized : Vector2.zero)) * minimumAsteroidSeperation;
                        }

                        spawnpoints[i] = a;
                    }
                }
            }

            if(relapse) {
                Debug.Log("Relapsing...");
            }
        }

        Debug.Log("Finished fitting asteroids");

        foreach(Vector2 v in spawnpoints) {
            GameObject asteroid = Instantiate(asteroidPrefab);
            asteroid.name = asteroidPrefab.name;
            asteroid.transform.parent = transform;
            asteroid.transform.localPosition = v;

            asteroid.GetComponent<AsteroidGenerator>().GenerateAsteroid(MathHelper.Rand(asteroidMinSides, asteroidMaxSides), MathHelper.Rand(asteroidMinRadius, asteroidMaxRadius));
            asteroid.GetComponent<Rigidbody2D>().mass = asteroid.GetComponent<BreakableObject>().GetArea() * asteroidDensity;
            asteroid.GetComponent<BreakableObject>().maxHealth = asteroid.GetComponent<Rigidbody2D>().mass * healthToMassRatio;
            asteroid.GetComponent<BreakableObject>().FormatBreakable();

            asteroid.GetComponent<Rigidbody2D>().angularVelocity = MathHelper.Rand(0f, 180f);
            asteroid.GetComponent<Rigidbody2D>().AddForce(new Vector2(
                MathHelper.Rand(minStartingSpeed, maxStartingSpeed) * (MathHelper.Rand(-1f, 1f) < 0 ? -1 : 1),
                MathHelper.Rand(minStartingSpeed, maxStartingSpeed) * (MathHelper.Rand(-1f, 1f) < 0 ? -1 : 1)
            ) * asteroid.GetComponent<Rigidbody2D>().mass);
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.green;

        Gizmos.DrawLine(transform.position + new Vector3(width / 2f, height / 2f), transform.position + new Vector3(-width / 2f, height / 2f));
        Gizmos.DrawLine(transform.position + new Vector3(width / 2f, -height / 2f), transform.position + new Vector3(-width / 2f, -height / 2f));
        Gizmos.DrawLine(transform.position + new Vector3(width / 2f, height / 2f), transform.position + new Vector3(width / 2f, -height / 2f));
        Gizmos.DrawLine(transform.position + new Vector3(-width / 2f, height / 2f), transform.position + new Vector3(-width / 2f, -height / 2f));
    }
}