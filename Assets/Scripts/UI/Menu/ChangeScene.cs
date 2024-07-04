using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{

    public static ChangeScene instance;



    public void scene_changer(string scene_name, string mapChoice){
        TutorialCheckpoints.playerInTutorial = false;
        MapLoader.map = mapChoice;
        SceneManager.LoadScene(scene_name);  
      
    }

    public void Tutorial(string scene_name){
        SceneManager.LoadScene(scene_name);
    }

    public void noMenu_change(string scene_name, string mapChoice){

        MapLoader.mapdefault = mapChoice;

        MapLoader.mapselected +=1;
     
        SceneManager.LoadScene(scene_name); 

    }

    public void quit(){
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }
}
