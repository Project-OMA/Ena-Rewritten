using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


public class HandVibration : MonoBehaviour
{
    private AudioSource alarme;
    public Rigidbody rb;
    private bool isColliding=false;

    public AudioSource Alarme { get => alarme; set => alarme = value; }

    void Update()
    {
        if (isColliding){
            print("Esta tocando");
        } else
        {
            
            print("Parou de tocar");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("map"))
        {   
            isColliding=true;
            Alarme.Play();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("map"))
        {
            isColliding=false;
            Alarme.Stop();
        }
    }
}
