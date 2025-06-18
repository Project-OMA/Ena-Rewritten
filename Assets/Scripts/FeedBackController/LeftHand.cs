using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.XR;
using System.Linq;

public class LeftHand : MonoBehaviour
{

    public HandFeedback handFeedback;
    
    public ControllerDetector controllerDetector;

    public static bool leftInside = false;


    void Start()
    {

        controllerDetector = GameObject.Find("ControllerDetector").GetComponent<ControllerDetector>();
        
        
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (!HandCheck.LeftHand)
        {
            HandCheck.LeftHand = true;
            handFeedback.HandleCollisionEnter(collision, "Left"); 
        }
        

    }
    
    private void OnCollisionStay(Collision collision)
    {
        ControllerDetector.frameCounterLeft++;

        if (ControllerDetector.frameCounterLeft % ControllerDetector.waitLeft == 0 && HandFeedback.innerFeedbackLeft && !leftInside)
        {
            controllerDetector.HandVariation();
            handFeedback.DetectControllerLeft();
            
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        handFeedback.DetectControllerLeft();
        ControllerDetector.canAlternateLeft = false;
        ControllerDetector.frameCounterLeft = 0;
        Debug.Log("LEFT");
        handFeedback.HandleCollisionExit(collision, "Left");

    }

    


    
}
