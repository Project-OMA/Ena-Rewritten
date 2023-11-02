using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FeedBackController : MonoBehaviour
{
    static public List<string> History = new List<string>();
    static public List<CollisionEvent> Collisions = new List<CollisionEvent>();
    public HandVibration HandVibration;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Collisions.RemoveAll(item => {
            try
            {
                ProcessCollisionItem(item);
                return !item.IsActive;
            }
            catch (Exception ex)
            {
                Debug.LogError($"An error occurred: {ex.Message}");
                return true;
            }
        });
    }
    void ProcessCollisionItem(CollisionEvent item)
    {
        if (item.IsActive)
        {
            Debug.Log("Playing");
            HandVibration.Alarme.Play();
            return;
        }
        Debug.Log("Stopping");
        History.Add($"Collision on: {item.WhatColide}, in hand: {item.HandCollision}");
        HandVibration.Alarme.Stop();
        return;
    }
}
