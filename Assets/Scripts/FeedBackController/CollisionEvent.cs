using System;

public class CollisionEvent
{
    private DateTime lastColliding;

    [CsvColumn("Start Colliding Time")]
    private DateTime startColliding;

    [CsvColumn("Total Time Colliding")]
    private TimeSpan timeColliding;
    private bool isColliding;

    [CsvColumn("Collided Object")]
    public string CollidedObject { get; }

    [CsvColumn("Collision Location on Player")]
    public string CollisionLocationOnPlayer { get; }
    public FeedbackTypeEnum[] FeedbackType { get; }
    public bool IsColliding
    {
        get
        {
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

    [CsvColumn("Current Time Colliding")]
    public TimeSpan TimeColliding
    {
        get { return IsColliding ? timeColliding + (DateTime.Now - lastColliding) : timeColliding; }
    }

    public CollisionEvent(string collidedObject, string collisionLocationOnPlayer, params FeedbackTypeEnum[] feedbackType)
    {
        CollidedObject = collidedObject;
        CollisionLocationOnPlayer = collisionLocationOnPlayer;
        FeedbackType = feedbackType;
        IsColliding = true;
        startColliding = DateTime.Now;
        timeColliding = TimeSpan.Zero;
    }
}
