
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Serialization; 
using UnityEngine.UI;     
using Meta.WitAi.TTS.Utilities; 
using System.IO;



    public class TTSManager : MonoBehaviour
    {
        public TTSSpeaker tTSSpeaker;
        public TextAsset file;

        private string text;

        



        public void TTSMenu(bool hasMenu){

            text = file.ToString();

            string[] collection = ClearString();

            if(hasMenu){


                tTSSpeaker.Speak(collection[0]);

            }else{
              
                tTSSpeaker.SpeakQueued(collection[1]);
            }  

        }

        public void MapLoaded(string mapName){

            tTSSpeaker.SpeakQueued(mapName);
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
