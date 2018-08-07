using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class ShipSelection : MonoBehaviour {
    public Player player;
    public GameObject selectionPanel;
    public ShipStatSpecifier statSpecifier;

    public float scale = 100f;
    public float initialHorizontalPad = 20f, initialVerticalPad = 20f;
    public float horizontalPad = 20f, verticalPad = 20f;
    public float textPad = 30f;
    public int columns = 4;

    public GameObject textPrefab;
    public GameObject[] shipPrefabs;

    private int selection = 0;
    private AudioSource selectSound;

    void Start() {
        selectSound = GetComponent<AudioSource>();

        int column = 0, row = 0;

        foreach(GameObject p in shipPrefabs) {
            GameObject o = Instantiate(p, Vector3.zero, transform.rotation, transform);

            foreach(ParticleSystem s in o.GetComponent<Ship>().trails) {
                Destroy(s.gameObject);
            }

            foreach(FighterWeapon s in o.GetComponent<Fighter>().weapons) {
                if(s.aimable) s.gameObject.transform.localRotation = Quaternion.identity;
                Destroy(s);
            }

            foreach(Component c in o.GetComponents(typeof(Component))) {
                if(c is Behaviour) (c as Behaviour).enabled = false;
            }

            o.transform.localPosition = GetPos(column, row);
            o.transform.localScale = (Vector3.right + Vector3.up) * scale + Vector3.forward;
            o.name = p.name;

            GameObject t = Instantiate(textPrefab, Vector3.zero, transform.rotation, transform);
            t.transform.localPosition = o.transform.localPosition + Vector3.up * textPad;

            string n = "";
            string[] sp = Regex.Split(p.name, @"(?<!^)(?=[A-Z])");
            for(int i = 0; i < sp.Length; i++) n += (i == 0 ? "" : " ") + sp[i];
            t.GetComponent<Text>().text = n;
            t.name = "Text for " + p.name;

            column++;
            if(column >= columns) {
                column = 0;
                row++;
            }
        }

        Select(false);
    }

    void Update() {
        if(DosMenu.isMenuOpen) {
            if(Input.GetKeyDown(KeyCode.LeftArrow)) {
                if(selection - 1 >= 0) selection--;
                Select();
            }

            if(Input.GetKeyDown(KeyCode.RightArrow)) {
                if(selection + 1 < shipPrefabs.Length) selection++;
                Select();
            }
            
            if(Input.GetKeyDown(KeyCode.DownArrow)) {
                if(selection + columns < shipPrefabs.Length) selection += columns;
                Select();
            }
            
            if(Input.GetKeyDown(KeyCode.UpArrow)) {
                if(selection - columns >= 0) selection -= columns;
                Select();
            }
        }
    }

    Vector3 GetPos(int column, int row) {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector3 center = new Vector3(-rectTransform.rect.width, rectTransform.rect.height) / 2f;

        return center + Vector3.right * initialHorizontalPad + Vector3.right * column * horizontalPad + Vector3.down * initialVerticalPad + Vector3.down * row * verticalPad;
    }

    public void SelectionLeft() {
        if(--selection < 0) selection = shipPrefabs.Length - 1;
        Select();
    }
    
    public void SelectionRight() {
        if(++selection >= shipPrefabs.Length) selection = 0;
        Select();
    }

    public void Select(bool playSound = true) {
        player.shipArchetype = shipPrefabs[selection];

        int row = selection / columns;
        int column = selection - row * columns;
        selectionPanel.transform.localPosition = GetPos(column, row);

        if(playSound) AudioManager.Play(selectSound);
        statSpecifier.Display(player.shipArchetype.GetComponent<Ship>());
    }
}