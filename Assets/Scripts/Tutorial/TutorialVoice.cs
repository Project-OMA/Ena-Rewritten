using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Serialization; 
using UnityEngine.UI;     
using Meta.WitAi.TTS.Utilities; 
using System.IO;
using UnityEngine.XR;
public class TutorialVoice : MonoBehaviour
{
    public TTSManager tTSManager;
    public InteractionController interactionController;
    public TextAsset file;
    private string text;
    public GameObject leftHandDevice;
    public GameObject rightHandDevice;

    public AudioSource ttsSource;

    private bool checkPoint = false;

    private bool activated = false;

    int i = 0;




    string[] collectionTutorial;

    private void Awake()
    {
        rightHandDevice.SetActive(false);
        leftHandDevice.SetActive(false);
        Physics.IgnoreLayerCollision(0, 0, true);
        
    }
    void Start()
    {
        text = file.ToString();

        collectionTutorial = ClearString();

        
        interactionController.enabled = false;

        
        
    }

    void Update(){


        if(!checkPoint){

            if(!ttsSource.isPlaying){
                Debug.Log("AAAAAAAAAAAAA");
                tTSManager.TTSTutorial(collectionTutorial[i]);
                activated = false;
            }else{
                checkPoint = true;
            }
        }else if(!ttsSource.isPlaying){
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
                

            
        }

        
    }

    #region Utility Methods

        private string[] ClearString(){

            text = file.ToString();

            string[] collection = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            foreach (var sub in collection) {
                Debug.Log(sub);
            }

            return collection;

        }


   

    


        #endregion
}
