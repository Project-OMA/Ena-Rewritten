using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
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

    public static int frameCounterRight = 0;

    public static int frameCounterLeft = 0;

    private const int StoppedFrameLimit = 4;

    public static float DistanceLeft = 0.0f;

    public static float DistanceRight = 0.0f;

    private float distanceLimitLeft = 0.0f;
    private float distanceLimitRight = 0.0f;
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
            ProcessHand(ref PreviousPositionLeft, leftController, ref canAlternateLeft, ref stoppedLeft, "Left", ref distanceLimitLeft);
        }

    }
    public void HandVariationRight()
    {

        if (!RightHand.rightInside)
        {
            ProcessHand(ref PreviousPositionRight, rightController, ref canAlternateRight, ref stoppedRight, "Right", ref distanceLimitRight);
        }

    }

    public void ProcessHand(ref Vector3 previousPosition, Transform controller, ref bool canAlternate, ref int stopped, string handLabel, ref float distanceLimit)
    {
        float distance = Vector3.Distance(previousPosition, controller.position);
        
        Debug.Log($"{handLabel} Movement: {distance}");


        if (distance > distanceThreshold)
        {
            distanceLimit += distance;
            stopped = 0;
            canAlternate = true;

            if (distanceLimit > 0.1f)
            {
                switch (handLabel)
                {
                    case "Left":
                        DistanceLeft = distance;
                        handFeedback.VibrationVariationLeft();
                        break;

                    case "Right":
                        DistanceRight = distance;
                        handFeedback.VibrationVariationRight();
                        break;

                }
                distanceLimit = 0.0f;

            }

            

        }
        else
        {

            stopped++;
        }

        if (stopped >= StoppedFrameLimit && canAlternate)
        {
            canAlternate = false;
            Debug.Log($"{handLabel} Stopped.");

            switch (handLabel)
            {
            case "Left":
                handFeedback.VibrationVariationLeft();
                break;

            case "Right":
                handFeedback.VibrationVariationRight();
                break;
                
            }
        }

        previousPosition = controller.position;
    }
}
