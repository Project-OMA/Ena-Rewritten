using UnityEngine;
using UI;

using System;

public class InteractionController : MonoBehaviour
{
    public float walkDistance = 1.5f;
    
    float stepPeriod = 0.25f;
    float nextStepTime = -1f;
    
    int wallhit;

    public float runSpeed = 30;
    public GameObject player;
    public GameObject cam;
    private Collider collider;
    private CharacterController controller;

    private FeedbackController feedbackController;

    Vector3 moveVector;

    public TTSManager ttsManager;

    bool toggleHit = false;

    private float runningInput()

    {
        ttsManager?.Speak("Currently Running!");
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

        Vector3 previousPos = player.transform.position;
        controller.Move(moveVector);

        Vector3 currentPos = player.transform.position;

        Vector3 desiredPos = moveVector+previousPos;

        wallDetect();

        

        feedbackController.handleStep();
    }

    private void startMovement() {
        feedbackController.handleMovementStart();
    }
    private void stopMovement() {
        feedbackController.handleMovementStop();
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
                    if(hit.collider.gameObject.name.Contains("Wall")) 
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
        controller = GetComponent<CharacterController>();
        feedbackController = GetComponent<FeedbackController>();
        ttsManager = GameObject.FindObjectOfType<TTSManager>();
    }

    void FixedUpdate()
    {
        // Get inputs
        float x = Input.GetAxis("Vertical");
        float y = Input.GetAxis("Horizontal");

        if (x == 0 && y == 0) {
            // Stop moving
            nextStepTime = -1;
            startMovement();
        } else {
            // Do first step
            if (nextStepTime == -1) {
                doStep();
                stopMovement();
            } else {
                // Repeat for following steps
                if (Time.time > nextStepTime) {
                    doStep();
                }
            }
        }

        

        

        


        
        
        
        //feedbackController.handleWallCollision(hit.gameObject, wallhit);
        //feedbackController.handleWallCollision(hit.gameObject, wallhit);
        //feedbackController.handleWallCollision(hit.gameObject, wallhit);

        

       

    }

    
}


