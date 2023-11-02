using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;





public class HandVibration : MonoBehaviour
{
    public AudioSource alarme;
    public Rigidbody rb;
    private bool isColliding=false;

    public AudioSource Alarme { get => alarme; set => alarme = value; }

    void Update()
    { }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Starting OnCollisionEnter");
        if(FeedBackController.Collisions.Select(x => x.WhatColide).Contains(collision.gameObject.tag))
        {
            var colisions = new CollisionEvent
            {
                IsActive = true,
                WhatColide = collision.gameObject.tag
            };
            FeedBackController.Collisions.Add(colisions);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        Debug.Log("Starting OnCollisionExit");
        string targetTag = collision.gameObject.tag;
        var itemToUpdate = FeedBackController.Collisions.FirstOrDefault(x => x.WhatColide == targetTag);

        if (itemToUpdate != null)
        {
            itemToUpdate.IsActive = false;
        }
    }
}
