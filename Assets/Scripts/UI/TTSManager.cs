// TTSManager.cs
using UnityEngine;

namespace UI
{
    public class TTSManager : MonoBehaviour
    {
        private AndroidJavaObject textToSpeech;

        private void InitializeTTS()
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            textToSpeech = new AndroidJavaObject("android.speech.tts.TextToSpeech", currentActivity, new TTSListener());

        }

        public void Speak(string text)
        {
            if (textToSpeech != null)
            {
                textToSpeech.Call("speak", text);
            }
        }

        // Add any other TTS-related methods here

        private void OnDestroy()
        {
            // Release any resources when the object is destroyed
            if (textToSpeech != null)
            {
                textToSpeech.Dispose();
            }
        }
    }

    class TTSListener : AndroidJavaProxy
{
    public TTSListener() : base("android.speech.tts.TextToSpeech$OnInitListener") { }

    // You can override other TextToSpeech listener methods here if needed
    void onInit(int status)
    {
        if (status == 0) // SUCCESS constant
        {
            Debug.Log("TextToSpeech initialized successfully");
        }
        else
        {
            Debug.LogError("TextToSpeech initialization failed");
        }
    }
}
}
