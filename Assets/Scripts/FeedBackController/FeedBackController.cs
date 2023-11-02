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
        if(!CollisionEventHandler.Collisions.Select(x => x.WhatCollide).Contains(collision.gameObject.tag))
        {
            var colisions = new CollisionEvent
            {
                IsActive = true,
                Playing = true,
                WhatCollide = collision.gameObject.tag,
                FeedbackType = FeedbackTypeEnum.Alarm
            };
            CollisionEventHandler.Collisions.Add(colisions);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        string targetTag = collision.gameObject.tag;
        var itemToUpdate = CollisionEventHandler.Collisions.FirstOrDefault(x => x.WhatCollide == targetTag);

        if (itemToUpdate != null)
        {
            itemToUpdate.IsActive = false;
        }
    }
}
