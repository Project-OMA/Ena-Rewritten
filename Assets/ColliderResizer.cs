using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class ColliderResizer : MonoBehaviour
{

    
    public InputActionProperty DecreaseButton;

    public InputActionProperty IncreaseButton;

    public Vector3 SizeUpdater = new Vector3(0.05f, 0.05f, 0.05f);

    private Vector3 minSize = new Vector3(0.8f, 0.8f, 0.8f);

    private Vector3 maxSize = new Vector3(1.2f, 1.2f, 1.2f);

    public AudioSource audioSource;

    public BoxCollider boxCollider;

    public static Vector3 boxSize;

    // Update is called once per frame
    void Update()
    {

        if ((DecreaseButton.action.WasPressedThisFrame() || Input.GetKeyDown(KeyCode.D)) &&

            boxCollider.size.x > minSize.x &&
            boxCollider.size.y > minSize.y &&
            boxCollider.size.z > minSize.z
        )
        {
            audioSource.pitch = 0.5f;
            audioSource.Play();
            Debug.Log("SIZEOFTHELAD:" + boxCollider.size);
            boxCollider.size = boxCollider.size - SizeUpdater;
            boxSize = boxCollider.size;
        }

        if ((IncreaseButton.action.WasPressedThisFrame() || Input.GetKeyDown(KeyCode.P)) &&

            boxCollider.size.x < maxSize.x &&
            boxCollider.size.y < maxSize.y &&
            boxCollider.size.z < maxSize.z
        )
        {
            audioSource.pitch = 1.0f;
            audioSource.Play();
            Debug.Log("SIZEOFTHELAD:" + boxCollider.size);
            boxCollider.size = boxCollider.size + SizeUpdater;
            boxSize = boxCollider.size;
        }
        
    }
}
