using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerDetector : MonoBehaviour
{
    public Transform rightController;
    public Transform leftController;

    public static int waitRight = 1;
    public static int waitLeft = 1;

    public Vector3 PreviousPositionLeft;

    public Vector3 PreviousPositionRight;

    public float distanceThreshold = 0.0f;
    public float angleThreshold = 10f;

    public int stoppedRight = 0;

    public int stoppedLeft = 0;

    public static bool canAlternateLeft = false;

    public static bool canAlternateRight = false;

    public HandFeedback handFeedback;

    public static int frameCounterRight = 0;

    public static int frameCounterLeft = 0;

    private const int StoppedFrameLimit = 10;
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
    public void HandVariationLeft()
    {
        if (!LeftHand.leftInside)
        {
            ProcessHand(ref PreviousPositionLeft, leftController, ref canAlternateLeft, ref stoppedLeft, "Left", ref waitLeft);
        }

    }
    public void HandVariationRight()
    {

        if (!RightHand.rightInside)
        {
            ProcessHand(ref PreviousPositionRight, rightController, ref canAlternateRight, ref stoppedRight, "Right", ref waitRight);
        }

    }

    public void ProcessHand(ref Vector3 previousPosition, Transform controller, ref bool canAlternate, ref int stopped, string handLabel, ref int wait)
    {
        float distance = Vector3.Distance(previousPosition, controller.position);
        Debug.Log($"{handLabel} Movement: {distance}");

        if (distance > distanceThreshold)
        {

            stopped = 0;
            canAlternate = true;

            switch (handLabel)
            {

                case "Left":
                    handFeedback.VibrationVariationLeft();
                    break;

                case "Right":
                    handFeedback.VibrationVariationRight();
                    break;
            }
            

            switch (distance)
            {

                case > 0.5f:

                    wait = 2;

                    break;

                case > 0.2f:

                    wait = 4;

                    break;

                default:

                    wait = 10;

                    break;

            }
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
