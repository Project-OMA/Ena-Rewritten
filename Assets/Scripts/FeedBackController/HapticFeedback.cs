using System.Collections;
using UnityEngine;
using UnityEngine.XR;
public class HapticFeedback
{
    private InputDevice xrController;
    private MonoBehaviour self;
    private bool isHapticFeedbackPlaying = false;
    private float stopDuration = 30f; // Stop haptic feedback after 30 seconds

    public HapticFeedback(InputDevice inputDevice, MonoBehaviour self)
    {
        this.xrController = inputDevice;
        this.self = self;
    }

    public void Play(float amplitude = 0.4f)
    {
        Debug.Log($"isHapticFeedbackPlaying: {isHapticFeedbackPlaying}");
        if (!isHapticFeedbackPlaying)
        {
            isHapticFeedbackPlaying = true;
            self.StartCoroutine(PlayHapticFeedback(amplitude));
        }
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
                        xrController.SendHapticImpulse(channel, amplitude);
                        yield return null;
                    }
                    yield return null;
                }
            }
        }
        else
        {
            Debug.Log($"xrcontroller: {xrController}");
        }

        Stop();
    }

    public void Stop()
    {
        isHapticFeedbackPlaying = false;
    }

    public bool isPlaying
    {
        get { return isHapticFeedbackPlaying; }
    }
}
