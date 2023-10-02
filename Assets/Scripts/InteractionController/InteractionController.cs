using UnityEngine;

public class InteractionController : MonoBehaviour
{
    public float walkSpeed = 1;

    public float runSpeed = 3;
    public GameObject cam;
    private Collider collider;
    private Rigidbody rigidbody;

    private float runningInput()
    {
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

        return moveVector.normalized;
    }

    void Start()
    {
        // Get capsule collider
        collider = GetComponent<CapsuleCollider>();
        rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Get move vector
        Vector3 moveVector = getMoveVector();
        var deltaTime = Time.deltaTime;
        var runningInput = this.runningInput();
        var speed = (walkSpeed * Mathf.Abs(runningInput - 1)) + (runSpeed * runningInput);
        var move = moveVector * speed;

        rigidbody.velocity = move;
    }
}


