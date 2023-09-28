using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionController : MonoBehaviour
{
    public float walkSpeed = 1;
    public GameObject cam;
    public Collider collider;

    private Vector3 getMoveVector()
    {
        float x = Input.GetAxis("Vertical");
        float y = Input.GetAxis("Horizontal");
        var control = new Vector3(y, x, 0);

        Vector3 right = cam.transform.right;
        Vector3 forward = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up).normalized;
        Vector3 moveVector = forward * control.y + right * control.x;
        moveVector.y = 0;

        return moveVector;
    }

    void Start()
    {
        // Get capsule collider
        collider = GetComponent<CapsuleCollider>();
    }

    void Update()
    {
        // Get move vector
        Vector3 moveVector = getMoveVector();
        var deltaTime = Time.deltaTime;

        // Move player
        Vector3 movement = moveVector * walkSpeed * deltaTime;
        transform.position += movement;

    }
}

