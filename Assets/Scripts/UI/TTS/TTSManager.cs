
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

        public AudioSource VoiceSpeaker; 

        
        public void thirdCollision(string material){
            tTSSpeaker.SpeakQueued(material);
        }




    }
