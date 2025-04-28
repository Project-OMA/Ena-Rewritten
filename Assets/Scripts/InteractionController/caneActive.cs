using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;




public class caneActive : MonoBehaviour
{
    public GameObject bengala;

    bool isItemActive = false;
    bool lastPrimaryButtonState = false;

    public InputActionProperty caneButton;

    public AudioSource m_MyAudioSource;

    void Start()
    {
        //Fetch the AudioSource from the GameObject
        m_MyAudioSource = GetComponent<AudioSource>();
        
    }

    // Start is called before the first frame update
    void Update()
    {
        
        if(caneButton.action.WasPressedThisFrame() || Input.GetKeyDown(KeyCode.B)){
            isItemActive = !isItemActive;
            bengala.SetActive(isItemActive);

            if (isItemActive) {
                TutorialCheckpoints.caneActive = true;
                m_MyAudioSource.pitch = 1.0f;
            } else {
                m_MyAudioSource.pitch = 0.5f;
            }
            m_MyAudioSource.Play();
        }
            
        
    }

    
}
