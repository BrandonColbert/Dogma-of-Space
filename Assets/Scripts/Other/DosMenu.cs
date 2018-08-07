using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DosMenu : MonoBehaviour {
    [HideInInspector] public static bool isMenuOpen;
    public Canvas canvas;

    public GameObject[] menuElements;
    public GameObject[] ingameElements;
    public MonoBehaviour[] ingameScripts;
    public Spawnable[] spawners;

    private bool gameExists;
    private AudioSource selectSound;

    void Start() {
        selectSound = GetComponent<AudioSource>();
        ToggleMenu(true);
    }

    void Update() {
        if(gameExists && Input.GetKeyDown(KeyCode.Escape)) {
            ToggleMenu(!isMenuOpen);
        }

        if(isMenuOpen) {
            if(Input.GetKeyDown(KeyCode.Return)) {
                ButtonStartLevel();
            } else if(Input.GetKeyDown(KeyCode.Backspace)) {
                ButtonRestart();
            }
        }
    }

    void ToggleMenu(bool value) {
        if(value) {
            canvas.worldCamera = Camera.main;
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
        } else {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }

        isMenuOpen = value;
        foreach(GameObject o in menuElements) o.SetActive(value);
        foreach(GameObject o in ingameElements) o.SetActive(!value);
        foreach(MonoBehaviour m in ingameScripts) m.enabled = !value;
    }

    public void ButtonStartLevel() {
        AudioManager.Play(selectSound);
        ToggleMenu(false);

        int currentWaveNumber = 0;

        foreach(Spawnable s in spawners) {
            if(s is SpawnWave) {
                currentWaveNumber = (s as SpawnWave).currentWaveNumber;
            }

            s.ClearSpawn();
        }

        foreach(Spawnable s in spawners) {
            if(s is SpawnWave) {
                (s as SpawnWave).currentWaveNumber = currentWaveNumber;
            }

            s.Spawn();
        }

        gameExists = true;
    }

    public void ButtonRestart() {
        AudioManager.Play(selectSound);
        ToggleMenu(false);

        foreach(Spawnable s in spawners) s.ClearSpawn();
        foreach(Spawnable s in spawners) s.Spawn();

        gameExists = true;
    }

    public void ButtonExit() {
        foreach(Spawnable s in spawners) s.ClearSpawn(); //TODO: REMOVE ON FOR BUILDS
        Application.Quit();
    }

    public void SliderVolume(Slider slider) {
        AudioManager.volumeModifier = slider.value;
    }
}