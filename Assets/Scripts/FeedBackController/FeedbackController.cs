using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.XR;

public class FeedbackController : MonoBehaviour
{
    #region Fields

    private static readonly Dictionary<string, CollisionEvent> Collisions = new Dictionary<string, CollisionEvent>();
    private readonly string fileName = $"{Directory.GetCurrentDirectory()}/PlayerLogs/feedback.csv";

    #endregion

    #region Properties

    public Dictionary<string, AudioSource> SoundSources { get; private set; }
    public Rigidbody Rb { get; set; }
    public InputDevice inputDevice;
    private HapticFeedback HapticImpulse;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        InitializeInputDevice();
        InitializeFeedbackComponents();
    }

    private void Update()
    {
        HandleCollisionFeedback();
    }

    private void OnCollisionEnter(Collision collision)
    {
        HandleCollisionEnter(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        HandleCollisionExit(collision);
    }

    private void OnApplicationQuit()
    {
        SaveCollisionDataToCsv();
    }

    #endregion

    #region Initialization

    private void InitializeInputDevice()
    {
        List<InputDevice> devices = new List<InputDevice>();
        var isRightHand = gameObject.name.ToLower().Contains("right");
        InputDevices.GetDevicesAtXRNode(isRightHand ? XRNode.RightHand : XRNode.LeftHand, devices);
        inputDevice = devices.Count > 0 ? devices[0] : default;
    }

    private void InitializeFeedbackComponents()
    {
        HapticImpulse = new HapticFeedback(inputDevice, this);
        SoundSources = new Dictionary<string, AudioSource>();
    }

    #endregion

    #region Collision Handling

    private void HandleCollisionFeedback()
    {
        foreach (var item in Collisions.Values)
        {
            if (item.CollidedObject.Contains("floor"))
            {
                HandleWalkSound(item);
            }
            HandleFeedback(item);
        }
    }

    private void HandleCollisionEnter(Collision collision)
    {
        string collidedObjectTag = GetObjectName(collision.gameObject);
        string playerColliderTag = GetObjectName(gameObject);

        var feedbackSettings = collision.gameObject.GetComponent<ObjectFeedbackSettings>()?.settings;

        if (Collisions.TryGetValue(collidedObjectTag + playerColliderTag, out var item))
        {
            item.IsColliding = true;
        }
        else
        {
            var collisionEvent = new CollisionEvent(
                collidedObject: collidedObjectTag,
                collisionLocationOnPlayer: playerColliderTag,
                feedbackSettings: feedbackSettings);
            Collisions.Add(collidedObjectTag + playerColliderTag, collisionEvent);
        }
    }

    private void HandleCollisionExit(Collision collision)
    {
        string collidedObjectTag = GetObjectName(collision.gameObject);
        string playerColliderTag = GetObjectName(gameObject);

        if (Collisions.TryGetValue(collidedObjectTag + playerColliderTag, out var itemToUpdate))
        {
            itemToUpdate.IsColliding = false;
            HandleFeedback(itemToUpdate);
        }
    }

    #endregion

    #region Feedback Handling

    private void HandleWalkSound(CollisionEvent item)
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
        foreach (var feedbackType in collision.FeedbackSettings?.feedbackTypes ?? new FeedbackTypeEnum[0])
        {
            switch (feedbackType)
            {
                case FeedbackTypeEnum.Sound:
                    PlaySoundFeedback(collision.FeedbackSettings.sound, collision);
                    break;
                case FeedbackTypeEnum.Haptic:
                    PlayHapticFeedback(collision.FeedbackSettings.hapticForce, collision.IsColliding && collision.CanPlay);
                    break;
                default:
                    break;
            }
        }
    }

    #endregion

    #region Feedback Methods

    private void PlaySoundFeedback(AudioClip sound, CollisionEvent collision)
    {
        var source = SoundSources.GetValueOrDefault(collision.FeedbackSettings.sound?.name);
        if (source is null)
        {
            source = gameObject.AddComponent<AudioSource>();
            SoundSources.Add(collision.FeedbackSettings.sound?.name, source);
        }

        if (collision.CanPlay && collision.IsColliding)
        {
            if (!source.isPlaying)
            {
                if (sound != null)
                {
                    source.clip = sound;
                }
                source.Play();
            }
        }
        else
        {
            source.Stop();
        }
    }

    private void PlayHapticFeedback(float hapticForce, bool isToPlay)
    {
        if (isToPlay)
        {
            if (!HapticImpulse.isPlaying)
            {
                HapticImpulse.Play(hapticForce);
            }
        }
        else
        {
            HapticImpulse.Stop();
        }
    }

    #endregion

    #region Utility Methods

    private string GetObjectName(GameObject gameObject)
    {
        return string.IsNullOrEmpty(gameObject.tag) ? gameObject.name : gameObject.tag + gameObject.name;
    }

    private void SaveCollisionDataToCsv()
    {
        CsvWriter.WriteToCsv(fileName, Collisions.Values);
    }

    #endregion
}
