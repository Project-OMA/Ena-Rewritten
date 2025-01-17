using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; 
using System.Linq;
using UnityEngine.UI;

public class MenuTrans : MonoBehaviour
{
    // Start is called before the first frame update
    bool hasMenu = false;

    bool active = true;

    public ChangeScene changeScene;

    string mapLoad = MapLoader.mapMenu;

    public InputActionProperty showButton;

    public InputActionProperty transButton;

    public InputActionProperty tutButton;

    public InputActionProperty blindnessButton;

    public InputActionProperty trafficButton;

    public GameObject menu;

    public GameObject controllerLeft;
    public GameObject controllerRight;

    public Camera camera;
    public GameObject camPos;

    public GameObject blindnessCanvas;

    private bool hasFilter = false;

    
    private XRInteractorLineVisual interactorLineVisualLeft;
    private XRInteractorLineVisual interactorLineVisualRight;

    public Material blindnessFilter;
    public float alphaValue = 0.0f;
    public Color blindnessColor;


    public void exit(){

        changeScene.quit();

    }

    public void mainMenu(){
        MapLoader.played = false;
        changeScene.scene_changer_menu("MainMenu", mapLoad);
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

        if (blindnessCanvas != null)
            {
                blindnessCanvas.SetActive(value: !blindnessCanvas.activeSelf);
                blindnessColor = blindnessFilter.color;
            }
            

        
        
        
        
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

        if(blindnessButton.action.WasPressedThisFrame()){



            blindnessColor.a = alphaValue;
            blindnessFilter.SetColor("_Color",blindnessColor);
            blindnessCanvas.SetActive(value: true);


            if(alphaValue>1.0f){

                alphaValue = 0.0f;
                blindnessCanvas.SetActive(value: false);

            }else if(alphaValue >= 0.9f){

                alphaValue+=0.05f;


            }else{
                alphaValue += 0.45f;
            }

            


        }

        if(trafficButton.action.WasPressedThisFrame() && !hasMenu && !MapLoader.playerInTraffic){

            changeScene.Traffic("TrafficTest");

        }


       

        if(showButton.action.WasPressedThisFrame() && !hasMenu && MapLoader.playerInMain){
            
            exit();
                
            
        }

        if((tutButton.action.WasPressedThisFrame() || Input.GetKeyDown(KeyCode.G)) && !hasMenu && !TutorialCheckpoints.playerInTutorial){

            
                
            TutorialCheckpoints.playerHasMoved = false;
            TutorialCheckpoints.playerHasInteracted = false;
            TutorialCheckpoints.caneActive = false;
            TutorialCheckpoints.playerOnTrigger = false;
            TutorialCheckpoints.playerDoor = false;
            MapLoader.played = false;
            
            TutorialCheckpoints.playerInTutorial = true;
            changeScene.Tutorial("TutorialScene");
            

            

        }

        if((showButton.action.WasPressedThisFrame()|| Input.GetKeyDown(KeyCode.Y)) && TutorialCheckpoints.playerInTutorial){


            if(hasMenu){
                changeScene.scene_changer_menu("MainMenu", mapLoad);

            }else{

                changeScene.noMenu_change("MainScene", MapLoader.beforeMap);

                }

            }

        if(showButton.action.WasPressedThisFrame() && MapLoader.playerInTraffic && !hasMenu){

            MapLoader.playerInTraffic = false;

            changeScene.noMenu_change("MainScene", MapLoader.beforeMap);

        }

            

        


        if((transButton.action.WasPressedThisFrame() || Input.GetKeyDown(KeyCode.M)) && !hasMenu  && MapLoader.playerInMain){

            MapLoader.mapselected +=1;
            
            if(MapLoader.mapselected<MapLoader.defaultMapList.Count){
                Debug.Log("NUMBER"+MapLoader.mapselected);
                Debug.Log("COUNT"+MapLoader.defaultMapList.Count);
                Debug.Log("MAP"+MapLoader.defaultMapList[MapLoader.mapselected]);

                MapLoader.mapNoMenu =  MapLoader.defaultMapList[MapLoader.mapselected];

                MapLoader.played = false;
                changeScene.noMenu_change("MainScene", MapLoader.mapNoMenu);

            }else{
                exit();
            }
        }

        menu.transform.position = camPos.transform.position+ new Vector3(x: camPos.transform.forward.x, y: 0, z: camPos.transform.forward.z).normalized;
        menu.transform.LookAt(worldPosition: new Vector3(x: camPos.transform.position.x, y: menu.transform.position.y, z: camPos.transform.position.z) );
        menu.transform.forward *=-1;
            
        
    }
}