using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wheelSpinner : MonoBehaviour
{
    // Update is called once per frame
    public GameObject wheel1;
    public GameObject wheel2;


    public float rotationSpeed = 100f; // Speed of rotation in degrees per second

    void Update()
    {
        if (trafficController.canMove)
        {
            RotateWheel(wheel1);

            RotateWheel(wheel2);
        }
    }

    void RotateWheel(GameObject wheel)
    {
        
        if (wheel.TryGetComponent(out Renderer renderer))
            {
                wheel.transform.RotateAround(renderer.bounds.center, Vector3.back, rotationSpeed);
            }
    }
}
