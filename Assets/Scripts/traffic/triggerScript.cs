using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class triggerScript : MonoBehaviour
{
    public AudioSource trafficSounds;

    private AudioClip clip;

    void Start(){

      trafficSounds = GameObject.Find("TrafficSounds").GetComponent<AudioSource>();

      clip = Resources.Load<AudioClip>("Sounds/idle");

    }

    
    void OnTriggerEnter(Collider collider)
    {
        Debug.Log("Triggered by: " + collider.gameObject.name);

        if (collider.CompareTag("car"))
        {

            trafficController.canMove = false;

            trafficSounds.Stop();
            trafficSounds.clip = clip;
            trafficSounds.Play();

        }
    }
}
