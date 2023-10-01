using UnityEngine;

public class MouserLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerBody;
    float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        xRotation += mouseX;
        transform.localRotation = Quaternion.Euler(0f, xRotation, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}