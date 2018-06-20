using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour {
    public GameObject target;
    public float minZoom = 2, maxZoom = 10, zoomIncrement = 0.25f;
    private float defaultZoom = 5f;

    void Start() {
        defaultZoom = GetComponent<Camera>().orthographicSize;
    }

    void LateUpdate() {
        if(target != null && target.activeSelf) {
            transform.position = new Vector3(target.transform.position.x, target.transform.position.y, transform.position.z);
        }
    }

    void Update() {
        Camera cam = GetComponent<Camera>();
        cam.orthographicSize -= Input.mouseScrollDelta.y * zoomIncrement;

        if(cam.orthographicSize < minZoom) {
            cam.orthographicSize = minZoom;
        } else if(cam.orthographicSize > maxZoom) {
            cam.orthographicSize = maxZoom;
        }

        if(Input.GetMouseButtonDown(2)) {
            cam.orthographicSize = defaultZoom;
        }
    }
}