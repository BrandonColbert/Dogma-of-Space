using UnityEngine;

public class Player : Spawnable {
    public string playerName = "Player";
    public CameraFollow cameraOnShip;
    public ShipStatusBar statusbar;
    public GameObject minimapIcon;
    public GameObject shipArchetype;

    [HideInInspector] public Ship ship;
    private Color specialColor;

    void Start() {
        specialColor = statusbar.specialTexture.color;
    }

    public override void ClearSpawn() {
        if(ship) ship.attributes.Kill();
    }

    public override void Spawn() {
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
        pfc.specialColor = specialColor;

        cameraOnShip.target = shipObject;
    }

    public bool isAlive() {
        return ship;
    }
}