using UnityEngine;

// This is a modification of the camera script made by Haravin (Daniel Valcour).
// This script is public domain, but credit is appreciated!
// Found at https://forum.unity.com/threads/free-third-person-camera-script.500496/

[RequireComponent(typeof(Camera))]
public class CameraSpectator : MonoBehaviour
{

    private float speed = 0.016f;
    private float mouseSensitivity = 1.5f;
    public bool invertMouse;

    private Camera cam;

    void Awake()
    {
        cam = gameObject.GetComponent<Camera>();
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * (Input.GetAxis("Mouse ScrollWheel") * 200 + Input.GetAxis("Vertical")));
        transform.Translate(Vector3.right * speed * Input.GetAxis("Horizontal"));
        if (Input.GetMouseButton(1)) // right click
        {
            transform.Translate(Vector3.right * speed * mouseSensitivity * 5 * Input.GetAxis("Mouse X"));
            transform.Translate(Vector3.up * speed * mouseSensitivity * 5 * Input.GetAxis("Mouse Y"));
        }
        if (Input.GetMouseButton(2)) // middle click
            transform.Rotate(Input.GetAxis("Mouse Y") * mouseSensitivity * ((invertMouse) ? 1 : -1), Input.GetAxis("Mouse X") * mouseSensitivity * ((invertMouse) ? -1 : 1), 0);
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0);
    }
}