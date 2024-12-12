using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Serialization; 
using UnityEngine.UI;      
using System.IO;
using UnityEngine.XR;
public class TutorialVoice : MonoBehaviour
{
    public InteractionController interactionController;
    public TextAsset file;
    private string text;
    public GameObject leftHandDevice;
    public GameObject rightHandDevice;
    public GameObject tutObj;

    public GameObject doorObj;

    public AudioSource tutorialSource;

    private bool checkPoint = false;

    private bool activated = false;

    int i = 0;

    public object[] collectionTutorial;

    void Start()
    {
    
        collectionTutorial = Resources.LoadAll("VoiceLines/TutorialLines", typeof(AudioClip));

        tutObj = GameObject.Find("FinalObject");
        doorObj = GameObject.Find("DoorGuide");


        rightHandDevice.SetActive(false);
        leftHandDevice.SetActive(false);
        tutObj.SetActive(false);
        doorObj.SetActive(false);
        Physics.IgnoreLayerCollision(0, 0, true);

        
        interactionController.enabled = false;
        tutorialSource.clip = (AudioClip)(collectionTutorial[i]);

        
        
    }

    void Update(){


        if(!checkPoint){

            Debug.Log("memebigBoy");

            if(!tutorialSource.isPlaying){
                Debug.Log("AAAAAAAAAAAAA");
                tutorialSource.clip = (AudioClip)(collectionTutorial[i]);
                tutorialSource.Play();
                activated = false;
            }else{
                Debug.Log("huh");
                checkPoint = true;
            }
        }else if(!tutorialSource.isPlaying){
            tutorialManager();
        }

    }

    private void tutorialManager(){

        switch(i){

            case 0:
                if(!TutorialCheckpoints.caneActive && !activated){
                    
                    rightHandDevice.SetActive(true);
                    leftHandDevice.SetActive(true);
                    activated = true;
            
                }else if(TutorialCheckpoints.caneActive && activated){
                    i++;
                    checkPoint = false;
                }

                break;

            case 1:

                if(!TutorialCheckpoints.playerHasInteracted && !activated){

                    
                    Physics.IgnoreLayerCollision(0, 0, false);
                    activated = true;
         
                }else if(TutorialCheckpoints.playerHasInteracted && activated){
                    i++;
                    checkPoint = false;
                }

                break;

            case 2:

                if(!TutorialCheckpoints.playerHasMoved && !activated){

                    
                    interactionController.enabled = true;
                    activated = true;
         
                }else if(TutorialCheckpoints.playerHasMoved && activated){
                    i++;
                    checkPoint = false;
                }

                break;

            case 3:

                if(!TutorialCheckpoints.playerOnTrigger && !activated){

                    
                    tutObj.SetActive(true);
                    activated = true;
         
                }else if(TutorialCheckpoints.playerOnTrigger && activated){

                    i++;
                    checkPoint = false;
                    tutObj.SetActive(false);

                }

                break;
            

            case 4:

                if(!TutorialCheckpoints.playerDoor && !activated){
                        doorObj.SetActive(true);
                        activated = true;
            
                    }else if(TutorialCheckpoints.playerDoor && activated){

                        i++;
                        doorObj.SetActive(false);
                        checkPoint = false;

                    }

                break;


                

            
        }

        
    }


}
