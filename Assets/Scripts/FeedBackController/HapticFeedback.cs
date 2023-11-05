using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HapticFeedback
{
    private XRController xrController;
    private bool isHapticFeedbackPlaying = false;
    private float hapticFeedbackAmplitude = 0.5f;
    private float hapticFeedbackDuration = 0.75f;
    private float stopDuration = 30f; // Stop haptic feedback after 30 seconds
    private float timer = 0f;

    public HapticFeedback(XRController xrController)
    {
        this.xrController = xrController;
    }

    public void Play()
    {
        if (!isHapticFeedbackPlaying)
        {
            isHapticFeedbackPlaying = true;
            xrController.StartCoroutine(PlayHapticFeedback());
        }
    }

    private IEnumerator PlayHapticFeedback()
    {
        while (isHapticFeedbackPlaying && timer < stopDuration)
        {
            xrController.SendHapticImpulse(hapticFeedbackAmplitude, hapticFeedbackDuration);
            timer += hapticFeedbackDuration;

            yield return new WaitForSeconds(hapticFeedbackDuration);
        }

        Stop();
    }

    public void Stop()
    {
        isHapticFeedbackPlaying = false;
    }

    public bool IsPlaying
    {
        get { return isHapticFeedbackPlaying; }
    }
}
