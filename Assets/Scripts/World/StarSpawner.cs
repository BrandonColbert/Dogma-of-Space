using UnityEngine;
using UnityEngine.UI;

public class StarSpawner : MonoBehaviour {
    public int rows = 10, columns = 10;
    public float textureScale = 1f;
    public int tileSize = 1000;
    public int minStarsPerTile, maxStarsPerTile;
    public Texture2D[] starPresets;
    
    void Start() {
        //for(int i = 0; i < rows; i++) for(int j = 0; j < columns; j++) {
            GameObject o = CreateTile(tileSize, tileSize);
            o.transform.parent = transform;
            o.transform.localPosition = Vector3.zero;

            o.GetComponent<SpriteRenderer>().drawMode = SpriteDrawMode.Tiled;
            
            Vector2 v = o.GetComponent<SpriteRenderer>().size;
            v.x *= rows;
            v.y *= columns;
            o.GetComponent<SpriteRenderer>().size = v;
        //}
    }

    GameObject CreateTile(int width, int height) {
        float mw = starPresets[0].width / 2f * textureScale;
        float mh = starPresets[0].height / 2f * textureScale;

        Vector2[] spawnpoints = new Vector2[MathHelper.Rand(minStarsPerTile, maxStarsPerTile)];

        for(int i = 0; i < spawnpoints.Length; i++) {
            spawnpoints[i] = new Vector2(MathHelper.Rand(mw, width - mw), MathHelper.Rand(mh, height - mh));
        }

        Texture2D stars = new Texture2D(width, height, TextureFormat.ARGB32, false);

        Color32[] cls = new Color32[width * height];
        for(int i = 0; i < width * height; i++) cls[i] = Color.clear;
        stars.SetPixels32(cls);

        for(int i = 0; i < spawnpoints.Length; i++) {
            Vector2 p = spawnpoints[i];   
            Texture2D t = starPresets[MathHelper.Rand(0, starPresets.Length - 1)];

            int w = Mathf.RoundToInt(t.width * textureScale);
            int h = Mathf.RoundToInt(t.height * textureScale);

            for(int j = 0; j < w; j++) {
                for(int k = 0; k < h; k++) {
                    int x = (int)(p.x + j - w / 2f);
                    int y = (int)(p.y + k - h / 2f);

                    if(0 <= x && x < width && 0 <= y && y < height) {
                        stars.SetPixel(x, y, stars.GetPixel(x, y) + t.GetPixel((int)(j / textureScale), (int)(k / textureScale)));
                    }
                }
            }

            //star.name = "Star";
            //star.transform.parent = transform;
            //star.transform.localPosition = spawnpoints[i];
            //star.transform.localEulerAngles = Vector3.forward * MathHelper.Rand(0f, 360f);
            //star.transform.localScale = (Vector3)(Vector2.one * MathHelper.Rand(0.1f, 1f)) + Vector3.forward;
        }

        stars.Apply();

        GameObject obj = new GameObject();
        obj.name = "StarField";
        obj.transform.localScale *= 10f;
        obj.AddComponent<SpriteRenderer>().sprite = Sprite.Create(stars, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));

        return obj;
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = new Color(0.8f, 0.8f, 0f);

        Gizmos.DrawLine(transform.position + new Vector3(rows * tileSize / 10f / 2f, columns * tileSize / 10f / 2f), transform.position + new Vector3(-rows * tileSize / 10f / 2f, columns * tileSize / 10f / 2f));
        Gizmos.DrawLine(transform.position + new Vector3(rows * tileSize / 10f / 2f, -columns * tileSize / 10f / 2f), transform.position + new Vector3(-rows * tileSize / 10f / 2f, -columns * tileSize / 10f / 2f));
        Gizmos.DrawLine(transform.position + new Vector3(rows * tileSize / 10f / 2f, columns * tileSize / 10f / 2f), transform.position + new Vector3(rows * tileSize / 10f / 2f, -columns * tileSize / 10f / 2f));
        Gizmos.DrawLine(transform.position + new Vector3(-rows * tileSize / 10f / 2f, columns * tileSize / 10f / 2f), transform.position + new Vector3(-rows * tileSize / 10f / 2f, -columns * tileSize / 10f / 2f));
    }
}