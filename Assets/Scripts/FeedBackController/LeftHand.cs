using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.XR;
using System.Linq;

public class LeftHand : MonoBehaviour
{

    public HandFeedback handFeedback;
    
    public ControllerDetector controllerDetector;

    void Start()
    {

        controllerDetector = GameObject.Find("ControllerDetector").GetComponent<ControllerDetector>();
        
        
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (!HandCheck.LeftHand)
        {
           HandCheck.LeftHand = true;
            handFeedback.HandleCollisionEnter(collision); 
        }
        

    }
    
    private void OnCollisionStay(Collision collision)
    {

        controllerDetector.HandVariation();
    }

    private void OnCollisionExit(Collision collision)
    {

        Debug.Log("LEFT");
        handFeedback.HandleCollisionExit(collision);

    }

    


    
}
