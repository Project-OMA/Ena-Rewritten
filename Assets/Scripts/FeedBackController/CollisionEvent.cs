public class CollisionEvent
{
    public string WhatCollide { get; set; }
    public string HandColliding { get; set; }
    public FeedbackTypeEnum FeedbackType { get; set; }
    public bool IsActive { get; set; }
    public bool Playing { get; set; }
}
