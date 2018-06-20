using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

public class ShipStatusBar : MonoBehaviour {
    public Image healthTexture, shieldsTexture, specialTexture;
    public float healthStart = 19f, healthEnd = 975f,
                 shieldsStart = 19f, shieldsEnd = 638f,
                 specialStart = 19f, specialEnd = 610f;

    void Start() {
        /*
        healthEnd = FindEnd(healthTexture.sprite.texture);
        shieldsEnd = FindEnd(shieldsTexture.sprite.texture);
        specialEnd = FindEnd(specialTexture.sprite.texture);
        */
    }

    float FindEnd(Texture2D tex) {
        //Debug.Log("Texture of size: " + tex.width + "x" + tex.height);
        for(int i = tex.width - 1; i >= 0; i--) {
            for(int j = 0; i < tex.height; j++) {
                //Debug.Log("At " + i + ", " + j + ": " + tex.GetPixel(i, j));
                Color c = tex.GetPixel(i, j);
                if(c.a > 0f && !(c.r == c.g && c.g == c.b && c.b == 1f)) {
                    return i;
                }
            }
        }

        return 0f;
    }

    public void SetHealth(float health, float max) {
        SetCutoff(healthTexture, (healthStart + health / max * healthEnd) / healthTexture.sprite.texture.width);
    }

    public void SetShields(float shields, float max) {
        SetCutoff(shieldsTexture, (shieldsStart + shields / max * shieldsEnd) / shieldsTexture.sprite.texture.width);
    }

    public void SetSpecial(float special, float max) {
        SetCutoff(specialTexture, (specialStart + special / max * specialEnd) / specialTexture.sprite.texture.width);
    }

    void SetCutoff(Image image, float cutoff) {
        Material m = Instantiate(image.material);
        m.SetFloat("_Cutoff", cutoff);
        image.material = m;
    }
}