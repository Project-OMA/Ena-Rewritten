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

    float original_size = 0.0f;
    float original_position = 0.0f;
    int caneExpansion = 0;

    void Start()
    {
        //Fetch the AudioSource from the GameObject
        m_MyAudioSource = GetComponent<AudioSource>();
        original_size = bengala.transform.localScale.y;
        original_position = bengala.transform.localPosition.z;
        
    }

    // Start is called before the first frame update
    void Update()
    {
        
        if((caneButton.action.WasPressedThisFrame() || Input.GetKeyDown(KeyCode.B)) && !HandFeedback.playerColliding){
            

            if (isItemActive && caneExpansion <3) {
                caneExpansion +=1;

                if(caneExpansion>1){

                    bengala.transform.localScale = new Vector3 (bengala.transform.localScale.x ,bengala.transform.localScale.y + 0.05f, bengala.transform.localScale.z);
                    bengala.transform.localPosition = new Vector3(0.0f, -0.019f, bengala.transform.localPosition.z -0.05f);


                }

                
                TutorialCheckpoints.caneActive = true;
                m_MyAudioSource.pitch = 1.0f;

            } else {
                caneExpansion = 0;
                bengala.transform.localScale = new Vector3 (bengala.transform.localScale.x, original_size, bengala.transform.localScale.z);
                bengala.transform.localPosition  = new Vector3(0.0f, -0.019f, original_position);
                isItemActive = !isItemActive;
                bengala.SetActive(isItemActive);
                m_MyAudioSource.pitch = 0.5f;
            }
            m_MyAudioSource.Play();
        }
            
        
    }

    
}
