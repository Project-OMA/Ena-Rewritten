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

    public void basic_change(string scene_name)
    {
        BasicSceneChanger.pos += 1;
        VibrationDetection.win = false;
        SceneManager.LoadScene(scene_name); 
    }



    public void scene_changer(string scene_name, string mapChoice)
    {
        MapLoader.isInMenu = false;
        collectLogs.SaveCollisionDataToCsv();
        TutorialCheckpoints.playerInTutorial = false;
        MapLoader.mapMenu = mapChoice;
        SceneManager.LoadScene(scene_name);

    }

    public void scene_changer_menu(string scene_name, string mapChoice){
        MapLoader.playerInTraffic = false;
        MapLoader.isInMenu = true;
        collectLogs.SaveCollisionDataToCsv();
        TutorialCheckpoints.playerInTutorial = false;
        MapLoader.mapMenu = mapChoice;
        SceneManager.LoadScene(scene_name);  
      
    }

    public void Tutorial(string scene_name){
        collectLogs.SaveCollisionDataToCsv();
        TutorialCheckpoints.playerHasMoved = false;
        TutorialCheckpoints.playerHasInteracted = false;
        TutorialCheckpoints.caneActive = false;
        TutorialCheckpoints.playerOnTrigger = false;
        TutorialCheckpoints.playerDoor = false;
        MapLoader.played = false;
        SceneManager.LoadScene(scene_name);
    }

    public void Traffic(string scene_name){
        
        MapLoader.playerInTraffic = true;
        SceneManager.LoadScene(scene_name);
        
    }

    public void noMenu_change(string scene_name, string mapChoice){

        collectLogs.SaveCollisionDataToCsv();

        TutorialCheckpoints.playerInTutorial = false;

        MapLoader.mapNoMenu = mapChoice;
     
        SceneManager.LoadScene(scene_name); 

    }

    public void quit(){
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }
}
