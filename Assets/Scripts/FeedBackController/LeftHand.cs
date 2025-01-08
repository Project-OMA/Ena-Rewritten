using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.XR;
using System.Linq;

public class LeftHand : MonoBehaviour
{
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Cane" || collision.gameObject.tag == "Left") return;
        Debug.Log("LEFT");
        HandCheck.LeftHand = true;
    }

    


    
}
