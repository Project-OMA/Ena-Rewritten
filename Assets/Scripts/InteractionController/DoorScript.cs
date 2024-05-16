using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject Door;

    public float xAngle, yAngle, zAngle;

    public bool DoorOpen;

    void OnTriggerEnter(Collider playCollider){

            if(playCollider.gameObject.tag == "Player" && DoorOpen == false){
                
                
                 
                openDoor();
            }  
        }

    void openDoor(){
        if(!DoorOpen){
            Door.transform.Rotate(xAngle, yAngle, zAngle, Space.Self); 
            DoorOpen = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }
}
