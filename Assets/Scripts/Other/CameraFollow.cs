using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour {
    public GameObject target;
    public CameraFollow alternateTarget;
    public bool zoomEnabled = true;
    public float minZoom = 2, maxZoom = 10, zoomIncrement = 0.25f;
    private float defaultZoom = 5f;
    public bool mouseCameraEnabled = true;
    public float mouseCameraDisplacement = 10f;
    private Vector3 mouseCameraDis;

    void Start() {
        defaultZoom = GetComponent<Camera>().orthographicSize;
    }

    void LateUpdate() {
        if(alternateTarget) {
            target = alternateTarget.target;
        }

        if(target != null && target.activeSelf) {
            Vector3 mousePos = Input.mousePosition;            
            mouseCameraDis = new Vector3(Mathf.Clamp(mousePos.x / Screen.width, 0f, 1f) - 0.5f, Mathf.Clamp(mousePos.y / Screen.height, 0f, 1f) - 0.5f) * 2f;

            transform.position = new Vector3(target.transform.position.x, target.transform.position.y, transform.position.z);
            if(mouseCameraEnabled) transform.position += mouseCameraDis * mouseCameraDisplacement;
        }
    }

    void Update() {
        if(zoomEnabled) {
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
}