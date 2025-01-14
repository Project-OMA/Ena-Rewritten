using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using System;

public class InteractionController : MonoBehaviour
{

    public static readonly Dictionary<int, PlayerEvent> PlayerDetects = new Dictionary<int, PlayerEvent>();

    public float walkDistance = 1.5f;
    
    float stepPeriod = 0.25f;
    float nextStepTime = -1f;
    
    int wallhit;

    public float runSpeed = 30;
    public Transform player;
    public GameObject Offset;
    public GameObject cam;
    private Collider collider;
    public Transform upperRayPos;
    public Transform lowerRayPos;

    public Transform xrOrigin;

    private string[] tagPrefab = {"Furniture", "Utensils", "Electronics", "Goals"};

    public FeedbackController feedbackController;

    Vector3 moveVector;

    bool toggleHit = false;
    private int nextUpdate=1;

    private float runningInput()

    {
        return Input.GetAxis("Fire2");
    }


    public Vector3 getMoveVector()
    {
        // Get player input
        float x = Input.GetAxis("Vertical");
        float y = Input.GetAxis("Horizontal");
        var control = new Vector3(y, x, 0);

        Vector3 right = cam.transform.right;
        Vector3 forward = cam.transform.forward;
        Vector3 moveVector = forward * control.y + right * control.x;
        moveVector.y = 0;

        
        return moveVector.normalized * walkDistance * stepPeriod;
    }


    private void doStep() {

        Debug.Log("Doing step at " + nextStepTime);
        moveVector = getMoveVector();
        
        nextStepTime = Time.time + stepPeriod;

        Vector3 previousPos = Offset.transform.position;
    
        if (moveVector != Vector3.zero && !headCollider.headcolliding)
        {

            Debug.DrawRay(lowerRayPos.position, moveVector.normalized, Color.red, 2, true);
            RaycastHit hit;
            // Raycast in the movement direction
            if (!Physics.Raycast(lowerRayPos.position, moveVector.normalized, out hit, 0.6f)){

                if (!Physics.SphereCast(upperRayPos.position, 0.4f, moveVector.normalized, out hit, maxDistance:0.2f))
                {
                
                    
                    player.Translate(moveVector, Space.World);
                    TutorialCheckpoints.playerHasMoved = true;
    

                    feedbackController.handleStep();

                    
                    
                    
                }else{

                    Debug.Log(hit.collider.gameObject.tag);

                    if(hit.collider.gameObject.tag == "Final" || hit.collider.gameObject.tag == "floor"|| hit.collider.gameObject.tag == "DoorWindow"){

                        player.Translate(moveVector, Space.World);
                        TutorialCheckpoints.playerHasMoved = true;
    

                        feedbackController.handleStep();

                    }

                    feedbackController.ObjectDectetorForCollision(hit.collider.gameObject, hit.point);

                    Debug.Log("Obstacle detected! Movement blocked.");
                }

            }else{

                Debug.Log(hit.collider.gameObject.tag);

                if(hit.collider.gameObject.tag == "Final" || hit.collider.gameObject.tag == "floor" || hit.collider.gameObject.tag == "DoorWindow"){

                        player.Translate(moveVector, Space.World);
                        TutorialCheckpoints.playerHasMoved = true;
    

                        feedbackController.handleStep();

                    }

                feedbackController.ObjectDectetorForCollision(hit.collider.gameObject, hit.point);

                Debug.Log("Obstacle detected! Movement blocked.");

            }
        

        }

        
    }

    void Start()
    {
        // Get capsule collider
        //player = GameObject.Find("Player");
        collider = GetComponent<CapsuleCollider>();
        feedbackController = GetComponent<FeedbackController>();

        
    }

    void Update(){
        
    	if(Time.time>=nextUpdate){

    		
    		nextUpdate=Mathf.FloorToInt(Time.time)+1;

            if(MapLoader.hasMenu){
                CheckPlayerEverySecond(player.position, MapLoader.mapMenu);
            }else{
                CheckPlayerEverySecond(player.position, MapLoader.mapNoMenu);
            }
    		
    		
    	}
    }

    void CheckPlayerEverySecond(Vector3 playerPos, string currentMap){

    var playerEvent = new PlayerEvent(
    
    
        vector3: playerPos,
        currentMap: currentMap
    );

    PlayerDetects.Add(nextUpdate, playerEvent);

    }

    void FixedUpdate()
    {
        // Get inputs
        float x = Input.GetAxis("Vertical");
        float y = Input.GetAxis("Horizontal");

        if (x == 0 && y == 0) {
            // Stop moving
            nextStepTime = -1;
            
        } else {
            // Do first step
            if (nextStepTime == -1) {
                doStep();
                
            } else {
                // Repeat for following steps
                if (Time.time > nextStepTime) {
                    doStep();
                }
            }
        }

        
        
    
        

    }



    
}


