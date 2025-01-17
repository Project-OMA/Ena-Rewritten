using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class adjustrotation : MonoBehaviour
{
    public Transform camOffset;

    public Transform collider;

    public Vector3 realPostion; 

    public Quaternion currentcam;

    
    void Update()
    {
        if (camOffset.rotation != currentcam){

            currentcam = camOffset.rotation;

            collider.rotation = Quaternion.Euler(camOffset.rotation.x, -camOffset.rotation.y, camOffset.rotation.z);

        }

        realPostion = collider.position;

        realPostion = new Vector3(camOffset.position.x, collider.position.y, camOffset.position.z);

        collider.position = realPostion;

        
        
    }
}
