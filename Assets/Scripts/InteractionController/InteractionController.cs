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

    public TTSManager ttsManager;

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
        Vector3 moveVector = getMoveVector();
        nextStepTime = Time.time + stepPeriod;

        Vector3 previousPos = player.transform.position;
        controller.Move(moveVector);

        Vector3 currentPos = player.transform.position;

        Vector3 desiredPos = moveVector+previousPos;

       

        Debug.Log("" + currentPos.x);
        Debug.Log("" + currentPos.z);

        Debug.Log(moveVector.x+previousPos.x);
        Debug.Log(moveVector.z+previousPos.z);

        diff(previousPos, desiredPos,currentPos, moveVector);

        

        feedbackController.handleStep();
    }

    private void startMovement() {
        feedbackController.handleMovementStart();
    }
    private void stopMovement() {
        feedbackController.handleMovementStop();
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
    }

    void OnControllerColliderHit(ControllerColliderHit hit) {

        
        feedbackController.handleWallCollision(hit.gameObject, wallhit);
    }

    public void diff(Vector3 prevPos, Vector3 desPos, Vector3 acPos, Vector3 movVec){


        

        if(Math.Abs(prevPos.x - desPos.x)<Math.Abs(movVec.x) || Math.Abs(prevPos.y - desPos.y)<Math.Abs(movVec.y)){
                wallhit=+1;
            }
            
        


        

    }
}


