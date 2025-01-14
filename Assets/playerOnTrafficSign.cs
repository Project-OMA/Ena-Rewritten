using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerOnTrafficSign : MonoBehaviour
{

    public static bool playerCrossing = false;
    // Start is called before the first frame update
    void OnTriggerEnter(Collider collision){

        if(collision.gameObject.tag == "Player"){

            

            playerCrossing = true;
        }


    }

    void OnTriggerExit(Collider collision){

        if(collision.gameObject.tag == "Player"){

            playerCrossing = false;
        }


    }
}
