using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class OculusPostProcessingSetup : MonoBehaviour
{
    public PostProcessVolume postProcessVolume;
    public Camera mainCamera;


    void Start()
    {
        // Ensure the camera has a PostProcessLayer attached
        postProcessVolume = GameObject.Find("postVolume")?.GetComponent<PostProcessVolume>();
        PostProcessLayer postProcessLayer = mainCamera.GetComponent<PostProcessLayer>();
        if (postProcessLayer == null)
        {
            postProcessLayer = mainCamera.gameObject.AddComponent<PostProcessLayer>();
        }

        
        postProcessLayer.volumeLayer = LayerMask.GetMask("Default"); 
        postProcessLayer.antialiasingMode = PostProcessLayer.Antialiasing.FastApproximateAntialiasing;

        
        if (postProcessVolume == null)
        {
            Debug.LogError("PostProcessVolume is missing!");
        }
        else
        {
            postProcessVolume.isGlobal = true;
        }
    }
}
