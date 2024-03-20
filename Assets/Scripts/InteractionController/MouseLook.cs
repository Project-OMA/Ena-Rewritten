using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 300f;
    GameObject camera;
    float xRotation = 0f;
    float yRotation = 0f;

    void Start()
    {
        camera = GameObject.FindWithTag("camera");
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        xRotation += mouseX;
        yRotation += mouseY;
        camera.transform.localRotation = Quaternion.Euler(-yRotation, xRotation, 0f);
    }
}