using System;
using UnityEngine;

public class CollisionEvent
{
    private DateTime lastColliding;

    [CsvColumn("Start Colliding Time")]
    public DateTime StartColliding { get; }

    private TimeSpan timeColliding;
    private bool isColliding;

    [CsvColumn("Collided Object")]
    public string CollidedObject { get; }
    public GameObject GameObject { get; }

    

    public int TotalCollisions { get; set; }


    [CsvColumn("What Collided")]
    public string Whatcollided { get; }
    public FeedbackSettings FeedbackSettings { get; }



    [CsvColumn("Collision Position")]
    public Vector3 Vector3 { get; set; }


    
    
    public bool IsColliding
    {
        get
        {
            if(DateTime.Now - lastColliding < TimeSpan.FromSeconds(1))
            {
                return true;
            }
            return isColliding;
        }
        set
        {
            if (value)
            {
                lastColliding = DateTime.Now;
                isColliding = true;
            }
            else if (isColliding)
            {
                timeColliding += DateTime.Now - lastColliding;
                isColliding = false;
            }
        }
    }

    public bool CanPlay { get; set; }
    public bool IsPlaying { get; set; }

    public bool IsRay { get; set; }

    [CsvColumn("Current Time Colliding")]
    public TimeSpan TimeColliding
    {
        get { return IsColliding ? timeColliding + (DateTime.Now - lastColliding) : timeColliding; }
    }

    public CollisionEvent(string collidedObject, string whatcollided, 
                            FeedbackSettings feedbackSettings, GameObject gameObject, 
                            Vector3 vector3, int totalCollisions)
    {
        CollidedObject = collidedObject;
        Whatcollided = whatcollided;
        FeedbackSettings = feedbackSettings;
        GameObject = gameObject;
        Vector3 = vector3;
        IsRay = false;
        IsColliding = true;
        CanPlay = true;
        StartColliding = DateTime.Now;
        timeColliding = TimeSpan.Zero;
        IsPlaying = false;
        TotalCollisions = totalCollisions;
    }
}
