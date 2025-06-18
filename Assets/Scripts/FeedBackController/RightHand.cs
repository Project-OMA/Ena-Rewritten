using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightHand : MonoBehaviour
{

    
    public HandFeedback handFeedback;
    public ControllerDetector controllerDetector;

    public static bool rightInside = false;

    void Start()
    {

        controllerDetector = GameObject.Find("ControllerDetector").GetComponent<ControllerDetector>();
        
        
    }


    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("rightcolen:" + HandCheck.RightHand);

        if (!HandCheck.RightHand)
        {
            HandCheck.RightHand = true;
            handFeedback.HandleCollisionEnter(collision, "Right");
        }
        
    }

    private void OnCollisionStay(Collision collision)
    {

        ControllerDetector.frameCounterRight++;
        if (ControllerDetector.frameCounterRight % ControllerDetector.waitRight == 0 && HandFeedback.innerFeedbackRight && !rightInside)
        {

            Debug.Log("rightcol:" + rightInside);
            controllerDetector.HandVariation();
            handFeedback.DetectControllerRight();

        }
        
    }


    private void OnCollisionExit(Collision collision)
    {

        handFeedback.DetectControllerRight();
        ControllerDetector.canAlternateRight = false;
        ControllerDetector.frameCounterRight = 0;
        Debug.Log("RIGHT");
        handFeedback.HandleCollisionExit(collision, "Right");

    }

    
}
