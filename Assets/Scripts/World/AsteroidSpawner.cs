//#define DEBUG_SPAWNS

using UnityEngine;

public class AsteroidSpawner : MonoBehaviour {
    [Tooltip("Used to load a basic unaltered asteroid")]
    public GameObject asteroidPrefab;
    [Tooltip("Area in which to spawn asteroids (Not guaranteed with a high asteroid count and low seperation distance)")]
    public float width = 100f, height = 100f;
    [Tooltip("Constraints on how many asteroids should be randomly generated")]
    public int minAsteroidCount = 10, maxAsteroidCount = 100;
    [Tooltip("Constraints for the amount of sides an asteroid can randomly be given")]
    public int asteroidMinSides = 5, asteroidMaxSides = 15;
    [Tooltip("Constraints on the the random radius of the asteroids")]
    public float asteroidMinRadius = 0.5f, asteroidMaxRadius = 5f;
    [Tooltip("Minimum distance that the asteroids must be from each other before being spawned")]
    public float minimumAsteroidSeperation = 12f;
    [Tooltip("Used in determing asteroid mass. Calculated with area * density")]
    public float asteroidDensity = 5f;
    [Tooltip("Health is determined by mass multiplied by this ratio")]
    public float healthToMassRatio = 1f;
    [Tooltip("The range of speed that an asteroid can be randomly given on first spawn")]
    public float minStartingSpeed = 0f, maxStartingSpeed = 250f;
    [Tooltip("Whether or not spawnpoints should be pushed outwards from the spawner's center to fit keep them within the asteroid seperation distance (May improve loading speed)")]
    public bool pushOutToFit = false;
    [Tooltip("When enabled, all the meshes are combined together to make one asteroid")]
    public bool clumpMeshes = false;

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
#if DEBUG_SPAWNS
                            Debug.Log("(" + (i * spawnpoints.Length + j) + "/" + (spawnpoints.Length * spawnpoints.Length) + ") Moving " + a + " away from " + b);
#endif
                            a += ((a - b).normalized + (pushOutToFit ? (a - (Vector2)transform.position).normalized : Vector2.zero)) * minimumAsteroidSeperation;
                        }

                        spawnpoints[i] = a;
                    }
                }
            }
#if DEBUG_SPAWNS
            if(relapse) {
                Debug.Log("Relapsing...");
            }
#endif
        }
#if DEBUG_SPAWNS
        Debug.Log("Finished fitting asteroids");
#endif

        CombineInstance[] supermesh = new CombineInstance[spawnpoints.Length];

        Transform parent = transform;
        while(parent.parent != null) parent = parent.parent;

        for(int i = 0; i < spawnpoints.Length; i++) {
            Vector2 v = spawnpoints[i];
            GameObject asteroid = Instantiate(asteroidPrefab);

            asteroid.name = asteroidPrefab.name;
            asteroid.transform.parent = transform;
            asteroid.transform.localPosition = v;
            asteroid.transform.SetParent(parent, true);

            asteroid.GetComponent<AsteroidGenerator>().GenerateAsteroid(MathHelper.Rand(asteroidMinSides, asteroidMaxSides), MathHelper.Rand(asteroidMinRadius, asteroidMaxRadius));

            if(clumpMeshes) {
                asteroid.transform.position -= transform.position;

                supermesh[i].mesh = asteroid.GetComponent<MeshFilter>().mesh;
                supermesh[i].transform = asteroid.transform.localToWorldMatrix;

                Destroy(asteroid);
                continue;
            }

            asteroid.GetComponent<Rigidbody2D>().mass = asteroid.GetComponent<BreakableObject>().GetArea() * asteroidDensity;
            asteroid.GetComponent<BreakableObject>().maxHealth = asteroid.GetComponent<Rigidbody2D>().mass * healthToMassRatio;
            asteroid.GetComponent<BreakableObject>().FormatBreakable();

            asteroid.GetComponent<Rigidbody2D>().angularVelocity = MathHelper.Rand(0f, 180f);
            asteroid.GetComponent<Rigidbody2D>().AddForce(new Vector2(
                MathHelper.Rand(minStartingSpeed, maxStartingSpeed) * (MathHelper.Rand(-1f, 1f) < 0 ? -1 : 1),
                MathHelper.Rand(minStartingSpeed, maxStartingSpeed) * (MathHelper.Rand(-1f, 1f) < 0 ? -1 : 1)
            ) * asteroid.GetComponent<Rigidbody2D>().mass);
        }

        if(clumpMeshes) {
            (gameObject.GetComponent<MeshRenderer>() ? gameObject.GetComponent<MeshRenderer>() : gameObject.AddComponent<MeshRenderer>()).materials = asteroidPrefab.GetComponent<MeshRenderer>().sharedMaterials;
            MeshFilter mf = gameObject.GetComponent<MeshFilter>() ? gameObject.GetComponent<MeshFilter>() : gameObject.AddComponent<MeshFilter>();
            mf.mesh = new Mesh();
            mf.mesh.CombineMeshes(supermesh);
            //(gameObject.GetComponent<BoxCollider2D>() ? gameObject.GetComponent<BoxCollider2D>() : gameObject.AddComponent<BoxCollider2D>()).size = new Vector2(width, height);
        }
    }

    void Outline() {
        Gizmos.DrawLine(transform.position + new Vector3(width / 2f, height / 2f), transform.position + new Vector3(-width / 2f, height / 2f));
        Gizmos.DrawLine(transform.position + new Vector3(width / 2f, -height / 2f), transform.position + new Vector3(-width / 2f, -height / 2f));
        Gizmos.DrawLine(transform.position + new Vector3(width / 2f, height / 2f), transform.position + new Vector3(width / 2f, -height / 2f));
        Gizmos.DrawLine(transform.position + new Vector3(-width / 2f, height / 2f), transform.position + new Vector3(-width / 2f, -height / 2f));
    }

    void OnDrawGizmos() {
        Gizmos.color = new Color(0.25f, 0.125f, 0f);
        Outline();
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = new Color(1f, 0.502f, 0f);
        Outline();
    }
}