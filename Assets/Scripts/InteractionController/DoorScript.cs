using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject Door;

    public float xAngle, yAngle, zAngle;

    public bool DoorOpen;

    public AudioSource doorSound; 

    public GameObject Parent;


    void OnTriggerEnter(Collider playCollider){

            if(playCollider.gameObject.tag == "Player" && DoorOpen == false){
                
                openDoor();

            }
            
        }

    
    void Start(){
        doorSound = Parent.GetComponent<AudioSource>();
    }
    
    

    void openDoor(){
        if(!DoorOpen){
            Door.transform.Rotate(xAngle, yAngle, zAngle, Space.Self); 
            DoorOpen = true;
            doorSound.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }
}
