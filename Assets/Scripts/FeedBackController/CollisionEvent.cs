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


    [CsvColumn("Collision Location on Player")]
    public string CollisionLocationOnPlayer { get; }
    public FeedbackSettings FeedbackSettings { get; }
    
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

    [CsvColumn("Current Time Colliding")]
    public TimeSpan TimeColliding
    {
        get { return IsColliding ? timeColliding + (DateTime.Now - lastColliding) : timeColliding; }
    }

    public CollisionEvent(string collidedObject, string collisionLocationOnPlayer, FeedbackSettings feedbackSettings, GameObject gameObject)
    {
        CollidedObject = collidedObject;
        CollisionLocationOnPlayer = collisionLocationOnPlayer;
        FeedbackSettings = feedbackSettings;
        GameObject = gameObject;
        IsColliding = true;
        CanPlay = true;
        StartColliding = DateTime.Now;
        timeColliding = TimeSpan.Zero;
        IsPlaying = false;
    }
}
