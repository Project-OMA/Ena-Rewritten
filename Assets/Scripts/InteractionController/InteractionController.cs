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
    public CharacterController controller;
    public GameObject warn;
    public GameObject sphere;
    public AudioSource warningSource;

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
        controller.Move(moveVector);

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
        warningSource.clip = (AudioClip)Resources.Load("VoiceLines/Warnings/Position");
        
    }

    void Update(){
        
    	if(Time.time>=nextUpdate){

    		
    		nextUpdate=Mathf.FloorToInt(Time.time)+1;

            if(MapLoader.hasMenu){
                CheckPlayerEverySecond(player.position, MapLoader.map);
            }else{
                CheckPlayerEverySecond(player.position, MapLoader.mapdefault);
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
        
        Vector3 OffsetCam = new Vector3 ((xrOrigin.transform.position.x - cam.transform.position.x), xrOrigin.transform.position.y, (xrOrigin.transform.position.z-cam.transform.position.z));
        
        //Debug.Log(xrOrigin.transform.position + " " + cam.transform.position + " " + OffsetCam.magnitude);

        if(OffsetCam.magnitude > 0.75 && !warn.activeSelf)
        {
            Debug.Log("Resetting camera position");

            
            warn.SetActive(true);
            sphere.SetActive(true);
            
            controller.enabled=false;
            //float newPosX = cam.transform.position.x;
            //float newPosZ = cam.transform.position.z;

            //controller.Move(new Vector3(OffsetCam.x, 0, OffsetCam.z));

            //xrOrigin.transform.position = new Vector3(newPosX, xrOrigin.transform.position.y, newPosZ);

            //cam.transform.localPosition = new Vector3(0, cam.transform.position.y, 0);
            warningSource.Play();
        

        }else if(OffsetCam.magnitude < 0.6){
            warn.SetActive(false);
            sphere.SetActive(false);
            controller.enabled=true;
            warningSource.Stop();
        }

        warn.transform.position = cam.transform.position+ new Vector3(x: cam.transform.forward.x, y: 0, z: cam.transform.forward.z).normalized;
        warn.transform.LookAt(worldPosition: new Vector3(x: cam.transform.position.x, y: warn.transform.position.y, z: cam.transform.position.z) );
        warn.transform.forward *=-1;
        
    
        

    }



    
}


