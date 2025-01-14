using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stopPlayer : MonoBehaviour
{

    public HandFeedback handFeedback;

    void Start()
    {
        
        handFeedback = GameObject.Find("Right Controller").GetComponent<HandFeedback>();   
        
    }


    void OnTriggerEnter(Collider collision){

        if(collision.gameObject.tag == "Player"){

            handFeedback.TrafficHaptic();
        }


    }

    void OnTriggerExit(Collider collision){

        if(collision.gameObject.tag == "Player"){

            handFeedback.stopHaptic();
        }


    }
}
