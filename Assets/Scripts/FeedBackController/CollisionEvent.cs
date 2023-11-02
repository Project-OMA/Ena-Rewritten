using System;

public class CollisionEvent
{
    private DateTime startPlaying;
    private TimeSpan timePlaying;
    private bool isPlaying;

    public string CollidedObject { get; }
    public string CollisionLocationOnPlayer { get; }
    public FeedbackTypeEnum FeedbackType { get; }
    public bool IsColliding { get; set; }
    public bool IsPlaying
    {
        get
        {
            return isPlaying;
        }
        set
        {
            if (value)
            {
                startPlaying = DateTime.Now;
                isPlaying = true;
            }
            else if (isPlaying)
            {
                timePlaying += DateTime.Now - startPlaying;
                isPlaying = false;
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
	timePlaying = TimeSpan.Zero;
    }
}
