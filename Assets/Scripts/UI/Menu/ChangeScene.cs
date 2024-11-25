using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;

public class ChangeScene : MonoBehaviour
{

    public static ChangeScene instance;

    private readonly string fileName = $"{Directory.GetCurrentDirectory()}/PlayerLogs/feedback.csv";



    public void scene_changer(string scene_name, string mapChoice){
        collectLogs.SaveCollisionDataToCsv();
        TutorialCheckpoints.playerInTutorial = false;
        MapLoader.map = mapChoice;
        SceneManager.LoadScene(scene_name);  
      
    }

    public void Tutorial(string scene_name){
        collectLogs.SaveCollisionDataToCsv();
        SceneManager.LoadScene(scene_name);
    }

    public void noMenu_change(string scene_name, string mapChoice){

        if(!TutorialCheckpoints.playerInTutorial){
            MapLoader.mapselected +=1;
        }

        TutorialCheckpoints.playerInTutorial = false;

        MapLoader.mapdefault = mapChoice;
     
        SceneManager.LoadScene(scene_name); 

    }

    public void quit(){
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }
}
