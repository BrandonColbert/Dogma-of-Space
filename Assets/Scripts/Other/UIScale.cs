using UnityEngine;
using System.Collections;

public class UIScale : MonoBehaviour {
    public Canvas canvas;

    void Update() {     
        float scale = Mathf.Clamp(1f / canvas.transform.localScale.x / 25f, 0f, 1f);
     
        transform.localScale = new Vector3(scale, scale, 1f);
    }
}