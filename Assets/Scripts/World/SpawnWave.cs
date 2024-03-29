using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SpawnWave : Spawnable {
    [System.Serializable]
    public class Wave {
        public List<GameObject> enemyPrefabs;
        [HideInInspector]
        public List<GameObject> enemies;
    }

    [Tooltip("Area in which to spawn enemies")]
    public int width = 100, height = 100;
    public Text textField;
    public GameObject enemyMinimapIcon;
    public int currentWaveNumber = 0;
    public Player[] players;
    public Wave[] waves;

    private int totalEnemies, playersLeft;
    private bool isRestarting;

    void Start() {
        gameObject.SetActive(false);
    }

    public override void ClearSpawn() {
        gameObject.SetActive(false);
        foreach(Transform t in transform) Destroy(t.gameObject);
        currentWaveNumber = 0;
        playersLeft = 0;
        totalEnemies = 0;
    }

    public override void Spawn() {
        StartWave(currentWaveNumber);
        gameObject.SetActive(true);
    }

    bool OnPlayerDeath(Damageable player) {
        player.GetComponent<Ship>().RemoveWhenKilled(player);

        playersLeft--;

        return false;
    }

    void Update() {
        if(IsWaveComplete()) {
            if(HasNextWave()) {
                currentWaveNumber++;
                StartWave(currentWaveNumber);
            } else {
                textField.text = "All " + (currentWaveNumber + 1) + " Waves Cleared!";
                Destroy(gameObject);
            }
        } else if(!isRestarting && AllPlayersDead()) {
            StartCoroutine(RestartAfter(1));
        } else {
            SetWaveText();
        }
    }

    IEnumerator RestartAfter(float seconds) {
        isRestarting = true;
        yield return new WaitForSeconds(seconds);
        RestartWave(currentWaveNumber);
        isRestarting = false;
    }

    bool AllPlayersDead() {
        return playersLeft <= 0;
    }

    bool HasNextWave() {
        return currentWaveNumber < waves.Length - 1;
    }

    bool IsWaveComplete() {
        return transform.childCount == 0;
    }

    void SpawnPlayers() {
        playersLeft = 0;

        foreach(Player player in players) {
            if(!player.isAlive()) {
                player.transform.position = Vector2.zero;
                player.Spawn();
                player.ship.GetComponent<Damageable>().OnKill(OnPlayerDeath);
                playersLeft++;
            }
        }
    }

    void StartWave(int waveNumber) {
        totalEnemies = 0;

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
                enemy.GetComponent<Ship>().controller = enemy.AddComponent<EnemyShipAI>();
                enemy.GetComponent<Ship>().SetMinimap(enemyMinimapIcon);
                wave.enemies.Add(enemy);
                totalEnemies++;
            }
        }

        SpawnPlayers();
    }

    public void SetWaveText() {
        textField.text = "Wave " + (1 + currentWaveNumber) + " [" + transform.childCount + "/" + totalEnemies + "]";
    }

    void RestartWave(int waveNumber) {
        foreach(GameObject obj in waves[waveNumber].enemies) {
            if(obj && !obj.GetComponent<Ship>().attributes.isDead) {
                obj.GetComponent<Ship>().attributes.Kill();
            }
        }

        foreach(Player player in players) {
            if(player.isAlive()) player.ship.attributes.Kill();
        }

        StartWave(waveNumber);
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;

        Gizmos.DrawLine(transform.position + new Vector3(width / 2f, height / 2f), transform.position + new Vector3(-width / 2f, height / 2f));
        Gizmos.DrawLine(transform.position + new Vector3(width / 2f, -height / 2f), transform.position + new Vector3(-width / 2f, -height / 2f));
        Gizmos.DrawLine(transform.position + new Vector3(width / 2f, height / 2f), transform.position + new Vector3(width / 2f, -height / 2f));
        Gizmos.DrawLine(transform.position + new Vector3(-width / 2f, height / 2f), transform.position + new Vector3(-width / 2f, -height / 2f));
    }
}