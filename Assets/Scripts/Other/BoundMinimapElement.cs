using UnityEngine;

public class BoundMinimapElement : MonoBehaviour {
    public bool master = false;
    [Range(0f, 1f)]
    public float sizeDistRatio = 1f;
    private static BoundMinimapElement minimap;
    private Vector3 originalPosition;

    void Start() {
        if(master) {
            minimap = this;
        } else {
            originalPosition = transform.localPosition;
        }
    }

    void Update() {
        if(!master && minimap) {
            Camera cam = minimap.GetComponent<Camera>();
            float size = cam ? cam.orthographicSize * minimap.sizeDistRatio : 100f;

            Vector2 dif = transform.parent.position - minimap.transform.position;
            if(dif.magnitude > size) {
                float angle = Mathf.Atan2(dif.y, dif.x);
                transform.position = (Vector2)minimap.transform.position + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * size;
            } else {
                transform.localPosition = originalPosition;
            }
        }
    }
}