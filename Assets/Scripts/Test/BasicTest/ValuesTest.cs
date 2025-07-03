using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ValuesTest : MonoBehaviour
{
    // Start is called before the first frame update

    public TMP_InputField leftHaptic;
    private float leftHapticValue = 0.0f;

    public TMP_InputField rightHaptic;

    private float rightHapticValue = 0.0f;


    public TMP_InputField leftDistance;
    private float leftDistanceValue = 0.0f;

    public TMP_InputField rightDistance;

    private float rightDistanceValue = 0.0f;
    // Update is called once per frame

    void Update()
    {

        if (leftHapticValue != HandFeedback.HapticValueLeft)
        {
            leftHapticValue = HandFeedback.HapticValueLeft;
            leftHaptic.text = HandFeedback.HapticValueLeft.ToString();
        }

        if (rightHapticValue != HandFeedback.HapticValueRight)
        {
            rightHapticValue = HandFeedback.HapticValueRight;
            rightHaptic.text = HandFeedback.HapticValueRight.ToString();
        }

        if (leftDistanceValue != ControllerDetector.DistanceLeft)
        {
            leftDistance.text = ControllerDetector.DistanceLeft.ToString();
        }

        if (rightDistanceValue != ControllerDetector.DistanceRight)
        {
            rightDistance.text = ControllerDetector.DistanceRight.ToString();
        }

        
    }
}
