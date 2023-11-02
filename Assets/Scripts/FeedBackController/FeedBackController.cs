using System.Linq;
using UnityEngine;

public class FeedbackController : MonoBehaviour
{
    private Collider playerCollider;

    private void Start()
    {
        playerCollider = GetComponent<Collider>();
    }
    void Update()
    { }

    private void OnCollisionEnter(Collision collision)
    {
        if(!CollisionEventHandler.Collisions.Select(x => x.CollidedObject).Contains(collision.gameObject.tag))
        {
            var colisions = new CollisionEvent(
                collidedObject: collision.gameObject.tag,
                collisionLocationOnPlayer: "",
                feedbackType: FeedbackTypeEnum.Alarm);
            CollisionEventHandler.Collisions.Add(colisions);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        string targetTag = collision.gameObject.tag;
        var itemToUpdate = CollisionEventHandler.Collisions.FirstOrDefault(x => x.CollidedObject == targetTag);

        if (itemToUpdate != null)
        {
            itemToUpdate.IsColliding = false;
        }
    }
}
