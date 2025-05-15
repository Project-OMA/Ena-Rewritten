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

    public GameObject leftHand;

    public GameObject rightHand;

    public GameObject camera;

    bool isItemActive = false;
    bool lastPrimaryButtonState = false;

    public InputActionProperty caneButton;

    public AudioSource m_MyAudioSource;

    public bool transfered = false;

    public InputActionProperty caneTransferRight;

    public InputActionProperty caneTransferLeft;

    float original_size = 0.0f;
    float original_position = 0.0f;
    int caneExpansion = 0;

    float difference;

    void Start()
    {
        //Fetch the AudioSource from the GameObject
        camera = GameObject.Find("Camera Offset");

        leftHand = GameObject.Find("XR Controller Left(Clone)");


        m_MyAudioSource = GetComponent<AudioSource>();


        difference = bengala.transform.localScale.y - camera.transform.localPosition.y/3;
        Debug.Log("AAAAAAAAAAAA" + difference);
        original_position = bengala.transform.localPosition.z + difference;

        bengala.transform.localScale = new Vector3 (bengala.transform.localScale.x ,(camera.transform.localPosition.y/3), bengala.transform.localScale.z);
        bengala.transform.localPosition = new Vector3(0.0f, -0.019f, original_position);
        original_size = bengala.transform.localScale.y;
        
        
    }

    // Start is called before the first frame update
    void Update()
    {
        
        if((caneButton.action.WasPressedThisFrame() || Input.GetKeyDown(KeyCode.B)) && !HandFeedback.playerColliding){

            

            if (isItemActive && caneExpansion <3) {

                caneExpansion +=1;
                

                if(caneExpansion>0){

                    bengala.transform.localScale = new Vector3 (bengala.transform.localScale.x ,bengala.transform.localScale.y + 0.025f, bengala.transform.localScale.z);
                    bengala.transform.localPosition = new Vector3(0.0f, -0.019f, bengala.transform.localPosition.z -0.025f);


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

        if((caneTransferLeft.action.WasPressedThisFrame() || Input.GetKeyDown(KeyCode.L)) && !HandFeedback.playerColliding && isItemActive){

            if (!transfered)
            {
                bengala.tag = "Left";
                bengala.transform.SetParent(leftHand.transform, false);
                bengala.transform.localPosition = new Vector3(0.0f, -0.019f, original_position);
                transfered = true;
            }
        }

        if((caneTransferRight.action.WasPressedThisFrame() || Input.GetKeyDown(KeyCode.R)) && !HandFeedback.playerColliding && isItemActive){

            if (transfered)
            {
                bengala.tag = "Cane";
                bengala.transform.SetParent(rightHand.transform, false);
                bengala.transform.localPosition = new Vector3(0.0f, -0.019f, original_position);
                transfered = false;
            }
        }

       

    
            
        
    }

    
}
