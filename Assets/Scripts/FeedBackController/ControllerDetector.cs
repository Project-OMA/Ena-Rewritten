using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerDetector : MonoBehaviour
{
    public Transform rightController;
    public Transform leftController;

    public Vector3 PreviousPositionLeft;

    public Vector3 PreviousPositionRight;

    public float distanceThreshold = 0.0f;
    public float angleThreshold = 10f;

    public int stoppedRight = 0;

    public int stoppedLeft = 0;

    public static bool canAlternateLeft = false;

    public static bool canAlternateRight = false;

    public HandFeedback handFeedback;

    public int frameCounter = 0;

    private const int StoppedFrameLimit = 30;
    // Start is called before the first frame update
    void Start()
    {

        handFeedback = GameObject.Find("XR Origin (XR Rig)").GetComponent<HandFeedback>();

        rightController = GameObject.Find("Right Controller").transform;
        leftController = GameObject.Find("Left Controller").transform;

        PreviousPositionRight = rightController.transform.position;
        PreviousPositionLeft = leftController.transform.position;

    }

    // Update is called once per frame
    public void HandVariation()
    {

        if (HandFeedback.innerFeedbackLeft)
        {
            ProcessHand(ref PreviousPositionLeft, leftController, ref canAlternateLeft, ref stoppedLeft, "Left");
        }

        if (HandFeedback.innerFeedbackRight)
        {
            ProcessHand(ref PreviousPositionRight, rightController, ref canAlternateRight, ref stoppedRight, "Right");
        }

    }
    
    public void ProcessHand(ref Vector3 previousPosition, Transform controller, ref bool canAlternate, ref int stopped, string handLabel)
    {
        float distance = Vector3.Distance(previousPosition, controller.position);
        Debug.Log($"{handLabel} Movement: {distance}");

        if (distance > distanceThreshold)
        {
            stopped = 0; 
            canAlternate = true; 
            handFeedback.VibrationVariation();
        }
        else
        {
            stopped++;
        }

        if (stopped >= StoppedFrameLimit)
        {
            canAlternate = false;
            Debug.Log($"{handLabel} Stopped.");
        }

        previousPosition = controller.position;
    }
}
