using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 400f;
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
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime;
        xRotation += mouseX * mouseSensitivity;
        yRotation += mouseY * mouseSensitivity;
        camera.transform.localRotation = Quaternion.Euler(-yRotation, xRotation, 0f);
    }
}