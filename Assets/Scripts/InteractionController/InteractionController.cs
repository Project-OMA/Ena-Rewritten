using UnityEngine;
using UI;

public class InteractionController : MonoBehaviour
{
    public float walkDistance = 1.5f;
    
    float stepPeriod = 0.25f;
    float nextStepTime = -1f;

    public float runSpeed = 30;
    public GameObject cam;
    private Collider collider;
    private CharacterController controller;

    public TTSManager ttsManager;

    private float runningInput()

    {
        ttsManager?.Speak("Currently Running!");
        return Input.GetAxis("Fire2");
    }

    private Vector3 getMoveVector()
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
        controller.Move(moveVector);
    }

    void Start()
    {
        // Get capsule collider
        collider = GetComponent<CapsuleCollider>();
        controller = GetComponent<CharacterController>();
        ttsManager = GameObject.FindObjectOfType<TTSManager>();
    }

    void Update()
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

    void OnControllerColliderHit() {
        Debug.Log("Player collided with wall");
    }
}


