using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class adjustrotation : MonoBehaviour
{
    public Transform camOffset;

    public Transform collider;

    public Quaternion currentcam;

    // Update is called once per frame
    void Start(){

        currentcam = camOffset.rotation;

    }
    void Update()
    {

        if (camOffset.rotation != currentcam){

            currentcam = camOffset.rotation;

            collider.rotation = Quaternion.Euler(-camOffset.rotation.x, -camOffset.rotation.y, -camOffset.rotation.z);

        }
        
    }
}
