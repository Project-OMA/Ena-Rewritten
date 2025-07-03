using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightTest : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnCollisionEnter(Collision collision)
    {

        VibrationDetection.canPress = true;

    }

    private void OnCollisionExit(Collision collision)
    {
        
        VibrationDetection.canPress = false;
        
    }
}
