using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightHand : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Cane" || collision.gameObject.tag == "Left") return;
        Debug.Log("RIGHT");
        HandCheck.RightHand = true;
    }

    
}
