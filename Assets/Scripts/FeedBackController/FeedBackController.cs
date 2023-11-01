using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FeedBackController : MonoBehaviour
{
    static public List<string> History = new List<string>();
    static public List<Collisions> Collisions = new List<Collisions>();
    public List<Collisions> CollisionPlaying = new List<Collisions>();
    public HandVibration HandVibration;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Collisions.Any(x => x.IsActive))
        {
            Debug.Log("Have collision");
            foreach (var item in Collisions)
            {
                Debug.Log("Playing");
                CollisionPlaying.Add(item);
                HandVibration.Alarme.Play();
            }
        }
        foreach(var item in CollisionPlaying.Except(Collisions.Where(x => x.IsActive)))
        {
            Debug.Log("Stoping");
            History.Add($"Collision on: {item.WhatColide}, in hand: {item.HandCollision}");
            HandVibration.Alarme.Stop();
        }
    }
}
