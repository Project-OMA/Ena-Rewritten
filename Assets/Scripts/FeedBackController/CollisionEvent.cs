using System;

public class CollisionEvent
{
    private DateTime startColliding;
    private TimeSpan timeColliding;
    private bool isColliding;

    public string CollidedObject { get; }
    public string CollisionLocationOnPlayer { get; }
    public FeedbackTypeEnum FeedbackType { get; }
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
                startColliding = DateTime.Now;
                isColliding = true;
            }
            else if (isColliding)
            {
                timeColliding += DateTime.Now - startColliding;
                isColliding = false;
            }
        }
    }

    public TimeSpan TimeColliding
    {
        get { return IsColliding ? timeColliding + (DateTime.Now - startColliding) : timeColliding; }
    }

    public CollisionEvent(string collidedObject, string collisionLocationOnPlayer, FeedbackTypeEnum feedbackType)
    {
        CollidedObject = collidedObject;
        CollisionLocationOnPlayer = collisionLocationOnPlayer;
        FeedbackType = feedbackType;
        IsColliding = true;
	    timeColliding = TimeSpan.Zero;
    }
}
