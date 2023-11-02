public class CollisionEvent
{
    public string WhatColide { get; set; }
    public string HandColliding { get; set; }
    public FeedbackTypeEnum FeedbackType { get; set; }
    public bool IsActive { get; set; }
    public override bool Equals(object obj)
    {
        return this.WhatColide.Equals(WhatColide);
    }
    public override int GetHashCode()
    {
        return WhatColide.GetHashCode();
    }
}
