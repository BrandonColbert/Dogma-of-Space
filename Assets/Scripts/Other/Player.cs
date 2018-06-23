using UnityEngine;

public class Player : MonoBehaviour {
    public string playerName = "Player";
    public bool spawnOnScriptStart = false;
    public CameraFollow cameraOnShip;
    public ShipStatusBar statusbar;
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
        ship.controller = shipObject.AddComponent<PlayerFighterController>();

        cameraOnShip.target = shipObject;
    }

    public bool isAlive() {
        return ship;
    }
}