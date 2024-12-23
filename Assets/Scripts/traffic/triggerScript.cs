using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class triggerScript : MonoBehaviour
{
    


    
    void OnTriggerEnter(Collider collider)
    {
        Debug.Log("Triggered by: " + collider.gameObject.name);

        if (collider.CompareTag("car"))
        {

            trafficController.canMove = false;

            

        }
    }
}
