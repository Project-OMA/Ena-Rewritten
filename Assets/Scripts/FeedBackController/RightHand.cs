using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightHand : MonoBehaviour
{

    
    public HandFeedback handFeedback;
    public ControllerDetector controllerDetector;

    void Start()
    {

        controllerDetector = GameObject.Find("ControllerDetector").GetComponent<ControllerDetector>();
        
        
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (!HandCheck.RightHand)
        {
            HandCheck.RightHand = true;
            handFeedback.HandleCollisionEnter(collision); 
        }
        
    }

    private void OnCollisionStay(Collision collision)
    {
        controllerDetector.HandVariation();
    }


    private void OnCollisionExit(Collision collision)
    {

        Debug.Log("RIGHT");
        handFeedback.HandleCollisionExit(collision);

    }

    
}
