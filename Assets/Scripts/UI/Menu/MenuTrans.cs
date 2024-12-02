using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; 
using System.Linq;

public class MenuTrans : MonoBehaviour
{
    // Start is called before the first frame update
    bool hasMenu = false;

    bool active = true;

    public ChangeScene changeScene;

    string mapLoad = MapLoader.map;

    public InputActionProperty showButton;

    public InputActionProperty transButton;

    public InputActionProperty tutButton;

    public InputActionProperty blindnessFilter;

    public GameObject menu;

    public GameObject controllerLeft;
    public GameObject controllerRight;

    public Camera camera;
    public GameObject camPos;

    public GameObject blindnessCanvas;

    private bool hasFilter = false;

    
    private XRInteractorLineVisual interactorLineVisualLeft;
    private XRInteractorLineVisual interactorLineVisualRight;




    public void exit(){

        changeScene.quit();

    }

    public void mainMenu(){
        changeScene.scene_changer("MainMenu", mapLoad);
    }


    void Awake()
    {

        hasMenu = MapLoader.hasMenu;

        changeScene = GameObject.Find("SceneManager").GetComponent<ChangeScene>();

        controllerLeft = GameObject.Find("Left Controller");
        controllerRight = GameObject.Find("Right Controller");


        camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        camPos = GameObject.Find("Main Camera");

        interactorLineVisualLeft = controllerLeft.GetComponent<XRInteractorLineVisual>();
        interactorLineVisualRight = controllerRight.GetComponent<XRInteractorLineVisual>();

        Canvas canvas = menu.GetComponent<Canvas>();

        canvas.worldCamera = camera;

    }

     void Start()
    {
        
        blindnessCanvas = GameObject.Find("blindness");
        blindnessCanvas.SetActive(value: !blindnessCanvas.activeSelf);
        
        
    }

   
    void Update()
    {
        
        if(showButton.action.WasPressedThisFrame() && hasMenu && !TutorialCheckpoints.playerInTutorial){

            Debug.Log("hallo");
            menu.SetActive(value: !menu.activeSelf);

            
            
            if(interactorLineVisualLeft.enabled && interactorLineVisualRight.enabled){

                interactorLineVisualLeft.enabled = false;
                interactorLineVisualRight.enabled = false;

            }else{
                interactorLineVisualLeft.enabled = true;
                interactorLineVisualRight.enabled = true;
            }
            
        
        }

        if(blindnessFilter.action.WasPressedThisFrame()){

            blindnessCanvas.SetActive(value: !blindnessCanvas.activeSelf);

        }


       

        if(showButton.action.WasPressedThisFrame() && !hasMenu && !TutorialCheckpoints.playerInTutorial){
            
            exit();
                
            
        }

        if(tutButton.action.WasPressedThisFrame() && !hasMenu && !TutorialCheckpoints.playerInTutorial){

            if(MapLoader.hasInternet){
                
                TutorialCheckpoints.playerHasMoved = false;
                TutorialCheckpoints.playerHasInteracted = false;
                TutorialCheckpoints.caneActive = false;
                TutorialCheckpoints.playerOnTrigger = false;
                TutorialCheckpoints.playerDoor = false;
                
                TutorialCheckpoints.playerInTutorial = true;
                changeScene.Tutorial("TutorialScene");
            }

            

        }

        if(showButton.action.WasPressedThisFrame() && TutorialCheckpoints.playerInTutorial){


            if(hasMenu){

                changeScene.scene_changer("MainMenu", mapLoad);

            }else{

                changeScene.noMenu_change("MainScene", MapLoader.mapdefault);

                }

            }

            

        


        if(transButton.action.WasPressedThisFrame() && !hasMenu  && !TutorialCheckpoints.playerInTutorial){
            
            if(MapLoader.mapselected<MapLoader.defaultMapList.Count){
                Debug.Log("NUMBER"+MapLoader.mapselected);
                Debug.Log("COUNT"+MapLoader.defaultMapList.Count);
                Debug.Log("MAP"+MapLoader.defaultMapList[MapLoader.mapselected]);

                MapLoader.mapdefault =  MapLoader.defaultMapList[MapLoader.mapselected];

                changeScene.noMenu_change("MainScene", MapLoader.mapdefault);

            }else{
                exit();
            }
        }

        menu.transform.position = camPos.transform.position+ new Vector3(x: camPos.transform.forward.x, y: 0, z: camPos.transform.forward.z).normalized;
        menu.transform.LookAt(worldPosition: new Vector3(x: camPos.transform.position.x, y: menu.transform.position.y, z: camPos.transform.position.z) );
        menu.transform.forward *=-1;
            
        
    }
}