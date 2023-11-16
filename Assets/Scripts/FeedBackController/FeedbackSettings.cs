using UnityEngine;

public class ObjectFeedbackSettings : MonoBehaviour
{
    public FeedbackSettings settings;
}

public class FeedbackSettings
{
    public AudioClip sound;
    public FeedbackTypeEnum[] feedbackTypes;
    public float hapticForce;
}
