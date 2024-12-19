using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cardespawner : MonoBehaviour
{
 public Transform carStart;

    void OnTriggerEnter(Collider collider)
    {

        if (collider.CompareTag("car"))
        {
            
            collider.gameObject.transform.position = new Vector3(
                carStart.position.x, 
                collider.transform.position.y, 
                carStart.position.z
            );

            Debug.Log("Car position reset to: " + collider.gameObject.transform.position);
        }
    }
}
