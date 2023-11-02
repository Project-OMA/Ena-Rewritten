using System;
using System.Collections.Generic;
using UnityEngine;

public class CollisionEventHandler : MonoBehaviour
{
    public AudioSource alarme { get; set; }
    public AudioSource Sound;
    public Rigidbody rb;
    public List<CollisionEvent> History = new List<CollisionEvent>();
    static public List<CollisionEvent> Collisions = new List<CollisionEvent>();

    private Dictionary<FeedbackTypeEnum, (Action, Action)> feedbackActions;

    private void Awake()
    {
        InitializeFeedbackActions();
    }

    private void InitializeFeedbackActions()
    {
        feedbackActions = new Dictionary<FeedbackTypeEnum, (Action, Action)>
        {
            { FeedbackTypeEnum.Alarm, (alarme.Play, alarme.Stop) },
            { FeedbackTypeEnum.Sound, (Sound.Play, Sound.Stop) },
        };
    }

    void Update()
    {
        Collisions.RemoveAll(item => {
            Debug.Log($"Collision with: {item.CollidedObject}");
            StartFeedback(item);
            return !item.IsColliding;
        });
    }

    private void StartFeedback(CollisionEvent collision)
    {
        if (feedbackActions.TryGetValue(collision.FeedbackType, out var actions))
        {
            var (play, stop) = actions;
            if (collision.IsColliding && !collision.IsPlaying)
            {
                play();
            }
            if (!collision.IsColliding)
            {
                stop();
                collision.IsPlaying = false;
                History.Add(collision);
            }
        }
    }
}
