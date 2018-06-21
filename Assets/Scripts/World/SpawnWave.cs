using UnityEngine;

public class SpawnWave : MonoBehaviour {
    [System.Serializable]
    public class Wave {
        public GameObject[] enemyPrefabs;
    }

    public int currentWave;
    public Wave[] waves;
    private int lastWave;

    void Start() {

    }

    void Update() {

    }

    void Progress() {
        Wave wave = waves[currentWave];

        if(wave) {

        }
    }
}