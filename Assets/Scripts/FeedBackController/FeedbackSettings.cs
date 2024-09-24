using UnityEngine;

public class ObjectFeedbackSettings : MonoBehaviour
{
    public FeedbackSettings settings;
}

public class FeedbackSettings
{
    public AudioClip sound1;
    public AudioClip sound2;
    public FeedbackTypeEnum[] feedbackTypes;
    public float hapticForce;
}
