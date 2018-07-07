using UnityEngine;

public class Player : MonoBehaviour {
    public string playerName = "Player";
    public bool spawnOnScriptStart = false;
    public CameraFollow cameraOnShip;
    public ShipStatusBar statusbar;
    public GameObject minimapIcon;
    public GameObject shipArchetype;

    [HideInInspector]
    public Ship ship;

    void Start() {
        if(spawnOnScriptStart) {
            Spawn();
        }
    }

    public void Spawn() {
        GameObject shipObject = Instantiate(shipArchetype);
        shipObject.transform.position = transform.position;
        shipObject.transform.rotation = transform.rotation;
        shipObject.transform.localScale = transform.localScale;
        shipObject.name = playerName + "\'s " + shipArchetype.name;

        ship = shipObject.GetComponent<Ship>();
        ship.statusBar = statusbar;
        ship.SetMinimap(minimapIcon);
        PlayerFighterController pfc = shipObject.AddComponent<PlayerFighterController>();
        pfc.player = this;
        ship.controller = pfc;

        cameraOnShip.target = shipObject;
    }

    public bool isAlive() {
        return ship;
    }
}