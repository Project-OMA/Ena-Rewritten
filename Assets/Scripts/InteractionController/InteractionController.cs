using UnityEngine;
using UI;

public class InteractionController : MonoBehaviour
{
    public float walkDistance = 1.5f;

    public float runSpeed = 3;
    public GameObject cam;
    private Collider collider;
    private Rigidbody rigidbody;

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
        Vector3 forward = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up).normalized;
        Vector3 moveVector = forward * control.y + right * control.x;
        moveVector.y = 0;

        return moveVector.normalized*walkDistance;
    }

    void Start()
    {
        // Get capsule collider
        collider = GetComponent<CapsuleCollider>();
        rigidbody = GetComponent<Rigidbody>();
        ttsManager = GameObject.FindObjectOfType<TTSManager>();
    }

    void Update()
    {
        // Get move vector
        Vector3 moveVector = getMoveVector();

        Debug.Log(transform.position);

        rigidbody.MovePosition(transform.position + moveVector);
        
    }
}


