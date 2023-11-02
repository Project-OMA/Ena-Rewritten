using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FeedBackController : MonoBehaviour
{
    public List<Collision> Collisions;
    public List<Collision> CollisionPlaing;
    public HandVibration HandVibration;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Collisions.Any())
        {
            foreach (var item in Collisions)
            {
                CollisionPlaing.Add(item);
                HandVibration.Alarme.Play();
            }
        }
        foreach(var item in CollisionPlaing.Except(Collisions))
        {
            HandVibration.Alarme.Stop();
        }
    }
}
