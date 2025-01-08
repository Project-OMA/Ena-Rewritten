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
    public Transform rayPos;

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
        float vertical = Input.GetAxis("Vertical");   // Forward/Backward
        float horizontal = Input.GetAxis("Horizontal"); // Left/Right

        // Combine input into a direction vector
        Vector3 inputDirection = new Vector3(horizontal, 0, vertical); // X = Left/Right, Z = Forward/Backward

        // Get camera directions
        Vector3 right = cam.transform.right;
        Vector3 forward = cam.transform.forward;

        // Ignore vertical component of the camera's forward vector
        forward.y = 0;
        forward.Normalize();
        right.y = 0; // Ensure the right vector is horizontal as well
        right.Normalize();

        // Calculate movement vector relative to the camera
        Vector3 moveVector = forward * inputDirection.z + right * inputDirection.x;

        // Scale movement vector by walkDistance and stepPeriod
        return moveVector.normalized * walkDistance * stepPeriod;
    }


    private void doStep() {

        Debug.Log("Doing step at " + nextStepTime);
        moveVector = getMoveVector();
        
        nextStepTime = Time.time + stepPeriod;

        Vector3 previousPos = Offset.transform.position;
    
        if (moveVector != Vector3.zero)
        {
            Debug.DrawRay(rayPos.position, moveVector.normalized, Color.red, 2, true);
            RaycastHit hit;
            // Raycast in the movement direction
            if (Physics.SphereCast(rayPos.position, 0.3f, moveVector, out hit, maxDistance:0.4f))
            {
                // Move the object if no obstacle is detected

                if (hit.collider != null) {

                    Debug.Log(hit.collider.gameObject.tag);
                    if (hit.collider.gameObject.tag == "floor" || hit.collider.gameObject.tag == "Player") {

                        Debug.Log("AAAAAAAAAAAAAAAAAAA");

                        moveVector = new Vector3(moveVector.z, 0, -moveVector.x);
                        player.Translate(moveVector);
                        TutorialCheckpoints.playerHasMoved = true;
        

                        feedbackController.handleStep();

                
                }else{
                    Debug.Log("Obstacle detected! Movement blocked.");
                }
                
            }
            
        }else
            {

                moveVector = new Vector3(moveVector.z, 0, -moveVector.x);
                player.Translate(moveVector);
                TutorialCheckpoints.playerHasMoved = true;
        

                feedbackController.handleStep();

                
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


