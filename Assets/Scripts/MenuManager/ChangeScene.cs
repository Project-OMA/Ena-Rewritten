using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{

    public static ChangeScene instance;



    public void scene_changer(string scene_name, string mapChoice){

        MapLoader.map = mapChoice;
     
        SceneManager.LoadScene(scene_name);  
      
    }
}
