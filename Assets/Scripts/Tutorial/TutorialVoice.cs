using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Serialization; 
using UnityEngine.UI;     
using Meta.WitAi.TTS.Utilities; 
using System.IO;
public class TutorialVoice : MonoBehaviour
{
    public TTSManager tTSManager;
    public InteractionController interactionController;
    public TextAsset file;
    private string text;

    string[] collectionTutorial;

    // Start is called before the first frame update
    void Start()
    {
        text = file.ToString();

        collectionTutorial = ClearString();

        tTSManager.TTSTutorial(collectionTutorial[0]);
        interactionController.enabled = false;
        tutorialManager();
    }

    private void tutorialManager(){
        if(!TutorialCheckpoints.caneActive){

            tTSManager.TTSTutorial(collectionTutorial[1]);

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
