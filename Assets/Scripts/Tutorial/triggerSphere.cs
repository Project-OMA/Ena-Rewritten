using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class triggerSphere : MonoBehaviour
{
    // Start is called before the first frame update
    void OnTriggerEnter(Collider playCollider){

        if(playCollider.gameObject.tag == "Player"){
                
                TutorialCheckpoints.playerOnTrigger = true;

            }
        
    }

}
