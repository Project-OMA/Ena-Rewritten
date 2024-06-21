using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterAllign : MonoBehaviour
{
    public CharacterController controller;
    private Transform vrHeadset; 

    private Vector3 moveDirection = Vector3.zero;

    void Start()
    {
        if (controller == null)
        {
            controller = GetComponent<CharacterController>();
        }

        vrHeadset = Camera.main.transform;

        if (vrHeadset == null)
        {
            Debug.LogError("Main camera (VR headset) not found!");
        }
    }

    void Update()
    {
        Vector3 targetPosition = new Vector3(vrHeadset.position.x, controller.transform.position.y, vrHeadset.position.z);
        Debug.Log("AAAAAAAAAAAAAAAAAAA"+vrHeadset.position);
        Debug.Log("aaa"+targetPosition);

       
        

        
        controller.transform.position = targetPosition;
    }
}
