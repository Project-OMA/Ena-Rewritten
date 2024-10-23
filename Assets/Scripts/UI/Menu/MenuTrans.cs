using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

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

    public GameObject menu;

    public Transform player;

    public GameObject controllerLeft;
    public GameObject controllerRight;

    public CharacterController controller;

    
    private XRInteractorLineVisual interactorLineVisualLeft;
    private XRInteractorLineVisual interactorLineVisualRight;

    public GameObject bengala;


    public void exit(){

        changeScene.quit();

    }

    public void mainMenu(){
        changeScene.scene_changer("MainMenu", mapLoad);
    }


    void Start()
    {

        hasMenu = MapLoader.hasMenu;

        changeScene = GameObject.Find("SceneManager").GetComponent<ChangeScene>();

        
        interactorLineVisualLeft = controllerLeft.GetComponent<XRInteractorLineVisual>();
        interactorLineVisualRight = controllerRight.GetComponent<XRInteractorLineVisual>();

    }

    // Update is called once per frame
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

        menu.transform.position = player.position+ new Vector3(x: player.forward.x, y: 0, z: player.forward.z).normalized*2;

       

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

        menu.transform.LookAt(worldPosition: new Vector3(x: player.position.x, y: menu.transform.position.y, z: player.position.z) );
        menu.transform.forward *=-1;
            
        
    }
}
