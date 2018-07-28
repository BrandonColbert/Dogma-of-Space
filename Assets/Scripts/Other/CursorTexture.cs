using UnityEngine;

public class CursorTexture : MonoBehaviour {
    public Texture2D texture;
    public Vector2 hotspot;
    public CursorMode mode = CursorMode.Auto;

    void Start() {
        hotspot = new Vector2(texture.width / 2, texture.height / 2);
        Cursor.SetCursor(texture, hotspot, mode);
    }
}