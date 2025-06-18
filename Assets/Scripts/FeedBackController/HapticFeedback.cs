using System.Collections;
using UnityEngine;
using UnityEngine.XR;
public class HapticFeedback
{
    private InputDevice xrController;
    private MonoBehaviour self;
    public bool isHapticFeedbackPlaying = false;
    private float stopDuration = 30f; // Stop haptic feedback after 30 seconds

    private float extra = 0.0f;

    public HapticFeedback(InputDevice inputDevice, MonoBehaviour self)
    {
        this.xrController = inputDevice;
        this.self = self;
    }

    public void Play(float amplitude)
    {
        // Debug.Log($"isHapticFeedbackPlaying: {isHapticFeedbackPlaying}");
        if (!isHapticFeedbackPlaying)
        {
            isHapticFeedbackPlaying = true;
            self.StartCoroutine(PlayHapticFeedback(amplitude));
        }
    }


    public void Adder(float adder)
    {
        Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAA"+ adder);
        extra = adder;

    }



    private IEnumerator PlayHapticFeedback(float amplitude)
    {
        if (xrController != null && xrController.TryGetHapticCapabilities(out var capabilities))
        {
            Debug.Log($"capabilities: {capabilities}");
            if (capabilities.supportsImpulse)
            {
                uint channel = 0;
                for (float elapsed = 0; elapsed < stopDuration; elapsed += Time.deltaTime)
                {
                    if(isHapticFeedbackPlaying)
                    {
                        
                        xrController.SendHapticImpulse(channel, amplitude + extra);
                        yield return null;
                    }
                    else {
                        xrController.StopHaptics();
                        yield break;
                    }
                }
            }
        }
        else
        {
            // Debug.Log($"xrcontroller: {xrController}");
        }

        Stop();
    }

    public void Stop()
    {
        extra = 0.0f;
        isHapticFeedbackPlaying = false;
    }

    public bool isPlaying
    {
        get { return isHapticFeedbackPlaying; }
    }
}
