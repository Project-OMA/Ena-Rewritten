using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class headCollider : MonoBehaviour
{

    public static bool headcolliding = false;
    // Start is called before the first frame update
    void OnCollisionEnter(Collision collision){
        

        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Cane" || collision.gameObject.tag == "Left" || collision.gameObject.tag == "DoorWindow" ) return;
        
        Debug.Log("AAAAAAAAAAA"+ collision.gameObject.name);

        headcolliding = true;

    }

    void OnCollisionExit(Collision collision){

        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Cane" || collision.gameObject.tag == "Left" || collision.gameObject.tag == "DoorWindow") return;

        headcolliding = false;
    }
}
