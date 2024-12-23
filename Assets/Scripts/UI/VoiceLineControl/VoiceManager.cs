using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class VoiceManager : MonoBehaviour
{
    public AudioSource audioSource; 

    public object[] VoiceLines;

    public Dictionary<string, AudioClip> audioClipsDict;

    private string loadedmap;

    private bool speak=false;



    // Start is called before the first frame update
    void Start()
    {

        if(MapLoader.hasMenu){
        
            VoiceLines = Resources.LoadAll("VoiceLines/MenuLines",typeof(AudioClip));
            

        }else{

            VoiceLines = Resources.LoadAll("VoiceLines/NoMenuMapLines",typeof(AudioClip));
            

        }

        loadedmap = "default";

        if(MapLoader.mapMenu == "default" && MapLoader.mapNoMenu !="default"){

            Debug.Log("_________"+MapLoader.mapNoMenu);

            loadedmap = MapLoader.mapNoMenu;

            
        }
        if(MapLoader.mapMenu != "default" && MapLoader.mapNoMenu =="default"){

            Debug.Log("_________"+MapLoader.mapMenu);

            loadedmap = MapLoader.mapMenu;
                
        }

        LoadAudioClipsToDictionary();

        loadedmap = RemoveFileExtension(loadedmap);

        Debug.Log("----"+loadedmap);

        if(loadedmap == "default"){
            MapLoader.isInMenu = true;
        }

        



    
    }

    // Update is called once per frame
    void Update()
    {


        if(!audioSource.isPlaying && MapLoader.lineCount < VoiceLines.Length){

            audioSource.clip = (AudioClip)VoiceLines[MapLoader.lineCount];

            MapLoader.lineCount+=1;

            audioSource.Play();

        }

        if(audioClipsDict.TryGetValue(loadedmap, out AudioClip myClip)){

            if(!speak && !MapLoader.isInMenu){
                speak = true;
                audioSource.clip = myClip;
                GetComponent<AudioSource>().Play(); 
            }

            

        }else{

            if(!speak && !MapLoader.isInMenu){
                speak = true;
                audioSource.clip = (AudioClip)Resources.Load("VoiceLines/MapList/NovoMapa");
                GetComponent<AudioSource>().Play();
            }
            
        }


        
        
    }

    #region utils
    private void LoadAudioClipsToDictionary()
    {
      
        audioClipsDict = new Dictionary<string, AudioClip>();

        
        AudioClip[] audioClips = Resources.LoadAll<AudioClip>("VoiceLines/MapList");

       
        foreach (AudioClip clip in audioClips)
        {
            if (!audioClipsDict.ContainsKey(clip.name))
            {
                audioClipsDict.Add(clip.name, clip);
            }
            else
            {
                Debug.LogWarning($"Duplicate AudioClip name found: {clip.name}. Skipping.");
            }
        }

        Debug.Log($"Loaded {audioClipsDict.Count} AudioClips into the dictionary.");
    }

     public static string RemoveFileExtension(string input)
    {
        
        if (input.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
        {
            return input.Substring(0, input.Length - 4); 
        }
        else if (input.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
        {
            return input.Substring(0, input.Length - 5); 
        }

    
        return input;
    }
    #endregion

}
