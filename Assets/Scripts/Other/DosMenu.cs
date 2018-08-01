using UnityEngine;

public class DosMenu : MonoBehaviour {
    [HideInInspector] public static bool isMenuOpen;
    public Canvas canvas;

    public GameObject[] menuElements;
    public GameObject[] ingameElements;
    public MonoBehaviour[] ingameScripts;
    public Spawnable[] spawners;

    private bool gameExists;

    void Start() {
        ToggleMenu(true);
    }

    void Update() {
        if(gameExists && Input.GetKeyDown(KeyCode.Escape)) {
            ToggleMenu(!isMenuOpen);
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

    public void ButtonStart() {
        ToggleMenu(false);
        foreach(Spawnable s in spawners) s.ClearSpawn();
        foreach(Spawnable s in spawners) s.Spawn();
        gameExists = true;
    }

    public void ButtonExit() {
        foreach(Spawnable s in spawners) s.ClearSpawn();
        //Application.Quit();
    }
}