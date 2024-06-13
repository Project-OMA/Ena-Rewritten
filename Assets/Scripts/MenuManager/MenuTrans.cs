using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuTrans : MonoBehaviour
{
    // Start is called before the first frame update
    bool hasMenu = false;

    public ChangeScene changeScene;

    string mapLoad = MapLoader.map;

    public InputActionProperty button;


    void Start()
    {

            if(mapLoad == "default"){

                hasMenu = false;

            }else{
                hasMenu = true;
            }
    }

    // Update is called once per frame
    void Update()
    {
        if (hasMenu){
            
            if(button.action.WasPressedThisFrame()){
                Debug.Log("hallo");
                changeScene.scene_changer("MainMenu", mapLoad);
            }
            
        }else{
            
            if(button.action.WasPressedThisFrame()){
                Debug.Log("hallo");
                changeScene.quit();
            }
        }
    }
}
