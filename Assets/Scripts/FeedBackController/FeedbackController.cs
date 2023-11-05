using System.Linq;
using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;
using System.IO;

public class FeedbackController : MonoBehaviour
{
    public AudioSource Alarme;
    public AudioSource Sound;
    public Rigidbody Rb { get; set; }
    private XRController xrController;
    private HapticFeedback haptic;
    static public List<CollisionEvent> History { get; } = new List<CollisionEvent>();
    private static readonly List<CollisionEvent> Collisions = new List<CollisionEvent>();

    private readonly string fileName = $"{Directory.GetCurrentDirectory()}/PlayerLogs/feedback.csv";

    private Dictionary<FeedbackTypeEnum, (Action, Action, Func<bool>)> feedbackActions;

    private void Awake()
    {
        xrController = GetComponent<XRController>();
        haptic = new HapticFeedback(xrController);
        Debug.Log($"tag{gameObject?.name ?? string.Empty}");
        InitializeFeedbackActions();
    }

    private void InitializeFeedbackActions()
    {
        feedbackActions = new Dictionary<FeedbackTypeEnum, (Action, Action, Func<bool>)>
        {
            { FeedbackTypeEnum.Alarm, (Alarme.Play, Alarme.Stop, () => Alarme.isPlaying) },
            { FeedbackTypeEnum.Sound, (Sound.Play, Sound.Stop, () => Sound.isPlaying) },
            { FeedbackTypeEnum.Haptic, (haptic.Start, haptic.Stop, () => haptic.IsPlaying) }
        };
    }

    private void Update()
    {
        foreach (var item in Collisions)
        {
            HandleFeedback(item);
        }
    }

    private void HandleFeedback(CollisionEvent collision)
    {
        if (feedbackActions.TryGetValue(collision.FeedbackType, out var actions))
        {
            var (play, stop, isPlaying) = actions;
            if (collision.IsColliding && !isPlaying())
            {
                Debug.Log($"Playing: {collision.CollidedObject}");
                play();
            }
            if(!collision.IsColliding)
            {
                Debug.Log($"Stopping: {collision.CollidedObject}");
                stop();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        string collidedObjectTag = GetObjectName(collision.gameObject);
        string playerColliderTag = GetObjectName(gameObject);

        if (!collidedObjectTag.Contains("floor") && !Collisions.Any(x => FilterCollision(x, collidedObjectTag, playerColliderTag)))
        {
            var collisionEvent = new CollisionEvent(
                collidedObject: collidedObjectTag,
                collisionLocationOnPlayer: playerColliderTag,
                feedbackType: FeedbackTypeEnum.Alarm);
            Collisions.Add(collisionEvent);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        string collidedObjectTag = GetObjectName(collision.gameObject);
        string playerColliderTag = GetObjectName(gameObject);

        var itemToUpdate = Collisions.FirstOrDefault(x => FilterCollision(x, collidedObjectTag, playerColliderTag));

        if (itemToUpdate != null)
        {
            itemToUpdate.IsColliding = false;
            HandleFeedback(itemToUpdate);
            History.Add(itemToUpdate);
            Collisions.Remove(itemToUpdate);
        }
    }

    private static bool FilterCollision(CollisionEvent x, string collidedObjectTag, string playerColliderTag)
    {
        return x.CollidedObject == collidedObjectTag && x.CollisionLocationOnPlayer == playerColliderTag;
    }

    private string GetObjectName(GameObject gameObject)
    {
        return (gameObject.tag == "Untagged"? "" : gameObject.tag) + gameObject.name;
    }

    void OnApplicationQuit()
    {
        CsvWriter.WriteToCsv(fileName, History);
    }
}
