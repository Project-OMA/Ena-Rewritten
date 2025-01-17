using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playSound : MonoBehaviour
{
    public AudioSource beep;

    private bool playing = false;

    // Update is called once per frame
    void Update()
    {

        if(trafficController.canMove && !playing){

            playing = true;
            beep.Play();

        }

        if(!trafficController.canMove && playing){

            playing = false;
            beep.Stop();

        }


        
    }
}
