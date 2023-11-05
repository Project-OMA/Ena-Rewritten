using UnityEngine.XR.Interaction.Toolkit;

public class HapticFeedback
{
    /*
        Class responsible for handling the haptic feedback.
    */
    private XRController xrController;
    public HapticFeedback(XRController xrController)
    {
        this.xrController = xrController;
    }
    private bool isHapticFeedbackPlaying = false;

    public void Start()
    {
        if (!isHapticFeedbackPlaying)
        {
            if (xrController != null)
            {
                xrController.SendHapticImpulse(0.5f, 0.75f);
            }
            isHapticFeedbackPlaying = true;
        }
    }

    public void Stop()
    {
        if (isHapticFeedbackPlaying)
        {
            isHapticFeedbackPlaying = false;
        }
    }

    public bool IsPlaying
    {
        get { return isHapticFeedbackPlaying; }
    }
}
