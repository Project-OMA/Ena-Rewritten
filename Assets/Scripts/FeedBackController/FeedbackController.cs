using System.Linq;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.XR;

public class FeedbackController : MonoBehaviour
{
    public AudioSource Alarme;
    public AudioSource Sound;
    public Rigidbody Rb { get; set; }
    public InputDevice inputDevice;
    private HapticFeedback HapticImpulse;
    static public List<CollisionEvent> History { get; } = new List<CollisionEvent>();
    private static readonly List<CollisionEvent> Collisions = new List<CollisionEvent>();

    private readonly string fileName = $"{Directory.GetCurrentDirectory()}/PlayerLogs/feedback.csv";

    private Dictionary<FeedbackTypeEnum, (Action, Action, Func<bool>)> feedbackActions;

    private void Awake()
    {
        // Initialize the InputDevice for the left Oculus Touch controller
        List<InputDevice> devices = new List<InputDevice>();
        var isRightHand = gameObject.name.ToLower().Contains("right");
        InputDevices.GetDevicesAtXRNode(isRightHand ? XRNode.RightHand : XRNode.LeftHand, devices);
        if (devices.Count > 0)
        {
            inputDevice = devices[0];
        }
        HapticImpulse = new HapticFeedback(inputDevice, this);
        InitializeFeedbackActions();
    }

    private void InitializeFeedbackActions()
    {
        feedbackActions = new Dictionary<FeedbackTypeEnum, (Action, Action, Func<bool>)>
        {
            { FeedbackTypeEnum.Alarm, (Alarme.Play, Alarme.Stop, () => Alarme.isPlaying) },
            { FeedbackTypeEnum.Sound, (Sound.Play, Sound.Stop, () => Sound.isPlaying) },
            { FeedbackTypeEnum.Haptic, (HapticImpulse.Play, HapticImpulse.Stop, () => HapticImpulse.IsPlaying) }
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
        foreach (var item in collision.FeedbackType)
        {
            if (feedbackActions.TryGetValue(item, out var actions))
            {
                var (play, stop, isPlaying) = actions;
                if (collision.IsColliding && !isPlaying())
                {
                    Debug.Log($"Playing: {item}");
                    play();
                }
                if(!collision.IsColliding)
                {
                    Debug.Log($"Stopping: {item}");
                    stop();
                }
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
                feedbackType: new [] { FeedbackTypeEnum.Haptic, FeedbackTypeEnum.Alarm });
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
