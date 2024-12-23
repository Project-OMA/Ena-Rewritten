using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cardespawner : MonoBehaviour
{
 public Transform carStart;

 public static int z;

 

    void OnTriggerEnter(Collider collider)
    {

        if (collider.CompareTag("car"))
        {
            
            int rand = Random.Range(-9, -6);

            if(rand == z){
                z-=1;
            }else{
                z = rand;
            }

            
            
            collider.gameObject.transform.position = new Vector3(
                carStart.position.x, 
                collider.transform.position.y, 
                z
            );

            Debug.Log("Car position reset to: " + collider.gameObject.transform.position);
        }
    }
}
