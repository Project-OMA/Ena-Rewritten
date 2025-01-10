using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class adjustrotation : MonoBehaviour
{
    public Transform camOffset;

    public Transform collider;

    public Vector3 realPostion; 

    
    void Update()
    {

        realPostion = collider.position;

        realPostion = new Vector3(camOffset.position.x, collider.position.y, camOffset.position.z);

        collider.position = realPostion;

        
        
    }
}
