using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightHand : MonoBehaviour
{

    
    public HandFeedback handFeedback;
    public ControllerDetector controllerDetector;

    //para o objectiveManager (verificando os objetos q o usuario tem interacao):
    private ObjectiveManager objectiveManager;

    public static bool rightInside = false;

    public static bool reducedRight = false;

    void Start()
    {

        controllerDetector = GameObject.Find("ControllerDetector").GetComponent<ControllerDetector>();
        objectiveManager = FindObjectOfType<ObjectiveManager>();
        
    }


    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("rightcolen:" + HandCheck.RightHand);

        if (!HandCheck.RightHand)
        {
            HandCheck.RightHand = true;
            handFeedback.HandleCollisionEnter(collision, "Right");
        }

        //para o objectiveManager
        if (objectiveManager != null)
        {
            objectiveManager.CheckGoalTouch(collision.gameObject);
        }
        
    }

    private void OnCollisionStay(Collision collision)
    {

        ControllerDetector.frameCounterRight++;
        if (ControllerDetector.frameCounterRight % 2 == 0 && HandFeedback.innerFeedbackRight && !rightInside)
        {
            HandFeedback.outRight = false;
            handFeedback.DetectControllerRight();
            controllerDetector.HandVariationRight();

        }
        
    }


    private void OnCollisionExit(Collision collision)
    {

        HandFeedback.outRight = true;
        handFeedback.DetectControllerRight();
        ControllerDetector.canAlternateRight = false;
        ControllerDetector.frameCounterRight = 0;
        Debug.Log("RIGHT");
        handFeedback.HandleCollisionExit(collision, "Right");

    }

    
}
