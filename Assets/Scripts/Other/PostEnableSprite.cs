using UnityEngine;

public class PostEnableSprite : MonoBehaviour {
    public SpriteRenderer spriteRenderer;
    
    void Awake() {
        if(spriteRenderer != null) spriteRenderer.enabled = true;
        Destroy(this);
    }
}