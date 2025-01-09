using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class adjustrotation : MonoBehaviour
{
    public Transform camOffset;

    public Transform collider;

    public Quaternion currentcam;

    public Vector3 realPostion; 

    // Update is called once per frame
    void Start(){

        currentcam = camOffset.rotation;

    }
    void Update()
    {

        realPostion = collider.position;
        

        if (camOffset.rotation != currentcam){

            currentcam = camOffset.rotation;

            collider.rotation = Quaternion.Euler(-camOffset.rotation.x, -camOffset.rotation.y, -camOffset.rotation.z);

            realPostion = new Vector3(camOffset.position.x, collider.position.y, camOffset.position.z);

            collider.position = realPostion;

        }
        
    }
}
