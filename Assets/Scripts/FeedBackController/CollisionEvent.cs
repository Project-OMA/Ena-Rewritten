using System;

public class CollisionEvent
{
    private DateTime startPlaying;
    private TimeSpan timePlaying;

    public string CollidedObject { get; }
    public string CollisionLocationOnPlayer { get; }
    public FeedbackTypeEnum FeedbackType { get; }
    public bool IsColliding { get; set; }
    public bool IsPlaying
    {
        get
        {
            return IsPlaying;
        }
        set
        {
            if (value)
            {
                startPlaying = DateTime.Now;
                IsPlaying = true;
            }
            else if (IsPlaying)
            {
                timePlaying += DateTime.Now - startPlaying;
                IsPlaying = false;
            }
        }
    }

    public TimeSpan TimePlaying
    {
        get { return IsPlaying ? timePlaying + (DateTime.Now - startPlaying) : timePlaying; }
    }

    public CollisionEvent(string collidedObject, string collisionLocationOnPlayer, FeedbackTypeEnum feedbackType)
    {
        CollidedObject = collidedObject;
        CollisionLocationOnPlayer = collisionLocationOnPlayer;
        FeedbackType = feedbackType;
        IsColliding = true;
        IsPlaying = false;
    }
}
