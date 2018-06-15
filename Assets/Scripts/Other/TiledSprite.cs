using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class TiledSprite : MonoBehaviour {
    public GameObject sprite;
    public int width, height;
    
    void OnValidate() {
        sprite.SetActive(true);

        Vector2 spriteDim = sprite.GetComponent<SpriteRenderer>().bounds.size;

        for(int x = 0; x < width; x++) {
            for(int y = 0; y < height; y++) {
                Vector2 tilePos = new Vector2(gameObject.transform.localPosition.x + x * spriteDim.x, gameObject.transform.localPosition.y + y * spriteDim.y);
                GameObject tile = Instantiate(sprite, tilePos, Quaternion.identity);
                tile.transform.parent = gameObject.transform;
            }
        }

        sprite.SetActive(false);
    }
}
