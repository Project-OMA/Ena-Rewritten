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
    private bool isWalking;
    static public List<CollisionEvent> History { get; } = new List<CollisionEvent>();
    private static readonly Dictionary<string, CollisionEvent> Collisions = new Dictionary<string, CollisionEvent>();

    private readonly string fileName = $"{Directory.GetCurrentDirectory()}/PlayerLogs/feedback.csv";

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
    }

    private void Update()
    {
        foreach (var item in Collisions.Values)
        {
            if(item.CollidedObject.Contains("floor"))
            {
                HanldeWalkSound(item);
            }
            HandleFeedback(item);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        string collidedObjectTag = GetObjectName(collision.gameObject);
        string playerColliderTag = GetObjectName(gameObject);

        var feedbackSettings = collision.gameObject.GetComponent<ObjectFeedbackSettings>().settings;

        if (Collisions.TryGetValue(collidedObjectTag+playerColliderTag, out var item))
        {
            item.IsColliding = true;
        }
        else
        {
            var collisionEvent = new CollisionEvent(
                collidedObject: collidedObjectTag,
                collisionLocationOnPlayer: playerColliderTag,
                feedbackSettings: feedbackSettings);
            Collisions.Add(collidedObjectTag+playerColliderTag, collisionEvent);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        string collidedObjectTag = GetObjectName(collision.gameObject);
        string playerColliderTag = GetObjectName(gameObject);

        if (Collisions.TryGetValue(collidedObjectTag+playerColliderTag, out var itemToUpdate))
        {
            itemToUpdate.IsColliding = false;
            HandleFeedback(itemToUpdate);
            History.Add(itemToUpdate);
            Collisions.Remove(collidedObjectTag+playerColliderTag);
        }
    }

    private void HanldeWalkSound(CollisionEvent item)
    {
        // Check if the player is moving (you can customize this based on your movement script)
        if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
        {
            if (!item.CanPlay)
            {
                item.CanPlay = true;
            }
        }
        else
        {
            if (item.CanPlay)
            {
                item.CanPlay = false;
            }
        }
    }
    private void HandleFeedback(CollisionEvent collision)
    {
        if (!collision.IsColliding || !collision.CanPlay)
        {
            StopAllFeedback();
            return;
        }

        foreach (var feedbackType in collision.FeedbackSettings.feedbackTypes)
        {
            switch (feedbackType)
            {
                case FeedbackTypeEnum.Sound:
                    PlaySoundFeedback(collision.FeedbackSettings.sound);
                    break;
                case FeedbackTypeEnum.Haptic:
                    PlayHapticFeedback(collision.FeedbackSettings.hapticForce);
                    break;
                default:
                    break;
            }
        }
    }

    private void PlaySoundFeedback(AudioClip sound)
    {
        if (!Sound.isPlaying)
        {
            Sound.clip = sound;
            Sound.Play();
        }
        else
        {
            Sound.Stop();
        }
    }

    private void PlayHapticFeedback(float hapticForce)
    {
        if (!HapticImpulse.isPlaying)
        {
            HapticImpulse.Play(hapticForce);
        }
        else
        {
            HapticImpulse.Stop();
        }
    }

    private void StopAllFeedback()
    {
        Sound.Stop();
        HapticImpulse.Stop();
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
