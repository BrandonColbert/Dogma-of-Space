using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnWave : MonoBehaviour {
    [System.Serializable]
    public class Wave {
        public List<GameObject> enemyPrefabs;
        [HideInInspector]
        public List<GameObject> enemies;
    }

    [Tooltip("Area in which to spawn enemies")]
    public float width = 100f, height = 100f;
    public int currentWaveNumber = 0;
    public Wave[] waves;

    void Start() {
        StartWave(currentWaveNumber);
    }

    void Update() {
        if(HasNextWave() && IsWaveComplete(currentWaveNumber)) {
            currentWaveNumber++;
            StartWave(currentWaveNumber);
        }
    }

    bool HasNextWave() {
        return currentWaveNumber < waves.Length - 1;
    }

    bool IsWaveComplete(int waveNumber) {
        waves[waveNumber].enemies.RemoveAll(o => o == null);

         return waves[waveNumber].enemies.Count() <= 0;
    }

    void StartWave(int waveNumber) {
        Wave wave = waves[waveNumber];
        Vector2[] spawnpoints = new Vector2[wave.enemyPrefabs.Count()];

        for(int i = 0; i < spawnpoints.Length; i++) {
            spawnpoints[i] = new Vector2(MathHelper.Rand(-width / 2f, width / 2f), MathHelper.Rand(-height / 2f, height / 2f));
        }

        for(int i = 0; i < wave.enemyPrefabs.Count(); i++) {
            GameObject prefab = wave.enemyPrefabs[i];
            if(prefab) {
                GameObject enemy = Instantiate(prefab);
                enemy.name = "Enemy " + (i + 1);
                enemy.transform.parent = transform;
                enemy.transform.localPosition = spawnpoints[i];
                enemy.transform.eulerAngles = Vector3.forward * MathHelper.Rand(0f, 360f);
                wave.enemies.Add(enemy);
            }
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;

        Gizmos.DrawLine(transform.position + new Vector3(width / 2f, height / 2f), transform.position + new Vector3(-width / 2f, height / 2f));
        Gizmos.DrawLine(transform.position + new Vector3(width / 2f, -height / 2f), transform.position + new Vector3(-width / 2f, -height / 2f));
        Gizmos.DrawLine(transform.position + new Vector3(width / 2f, height / 2f), transform.position + new Vector3(width / 2f, -height / 2f));
        Gizmos.DrawLine(transform.position + new Vector3(-width / 2f, height / 2f), transform.position + new Vector3(-width / 2f, -height / 2f));
    }
}