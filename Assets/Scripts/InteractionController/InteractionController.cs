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
            float vertical = Input.GetAxis("Vertical");   // Forward/Backward
            float horizontal = Input.GetAxis("Horizontal"); // Left/Right

            // Combine input into a direction vector
            Vector3 inputDirection = new Vector3(vertical, 0, -horizontal);

            Debug.Log(player.position);

            // Get camera directions
            Vector3 right = cam.transform.right;
            Vector3 forward = cam.transform.forward;

            // Ignore vertical component of the camera's forward vector
            forward.y = 0;
            forward.Normalize();

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
        player.Translate(moveVector);

        Vector3 currentPos = Offset.transform.position;

        Vector3 desiredPos = moveVector+previousPos;

        TutorialCheckpoints.playerHasMoved = true;

        wallDetect();

        

        feedbackController.handleStep();
    }


    private void wallDetect(){
        float raylen = 0.01f;
        Vector3 rayOffset = new Vector3(0,1,0);

        Vector3[] directions = new Vector3[] {
            transform.TransformDirection(Vector3.left),
            transform.TransformDirection(Vector3.right),
            transform.TransformDirection(Vector3.forward),
            transform.TransformDirection(Vector3.back)
        };

        Vector3 origin = transform.position + rayOffset;
        float radius = 0.15f; // Set the radius of your sphere cast

        foreach (var direction in directions)
        {
            Ray ray = new Ray(origin + direction, direction * raylen);
            RaycastHit hit;

            if (Physics.SphereCast(origin, radius, direction, out hit, maxDistance:0.15f))
            {
                Debug.DrawRay(origin, ray.direction, Color.yellow);

                if (hit.collider != null)
                {
                    if(hit.collider.gameObject.tag == "wall" || tagPrefab.Contains(hit.collider.gameObject.tag))
                    {
                        feedbackController.handleWallCollision(toggleHit);
                        toggleHit = true;
                        Debug.Log("PAREDE");
                        break;
                    }
                    else if(toggleHit) 
                    {
                        
                        toggleHit = false;
                    }
                }
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
        
        
        //Debug.Log(xrOrigin.transform.position + " " + cam.transform.position + " " + OffsetCam.magnitude);
        
    
        

    }



    
}


