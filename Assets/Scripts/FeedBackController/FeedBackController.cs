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
        Debug.Log("Starting OnCollisionEnter");
        if(CollisionEventHandler.Collisions.Select(x => x.WhatColide).Contains(collision.gameObject.tag))
        {
            var colisions = new CollisionEvent
            {
                IsActive = true,
                WhatColide = collision.gameObject.tag,
                FeedbackType = FeedbackTypeEnum.Alarm
            };
            CollisionEventHandler.Collisions.Add(colisions);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        Debug.Log("Starting OnCollisionExit");
        string targetTag = collision.gameObject.tag;
        var itemToUpdate = CollisionEventHandler.Collisions.FirstOrDefault(x => x.WhatColide == targetTag);

        if (itemToUpdate != null)
        {
            itemToUpdate.IsActive = false;
        }
    }
}
