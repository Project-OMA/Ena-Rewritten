using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.XR;
using System.Linq;

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
<<<<<<< Updated upstream
            if (item.CollidedObject.Contains("floor"))
            {
                HandleWalkSound(item);
            }else{
                Debug.LogWarning(item.CollidedObject);
            }
            HandleFeedback(item);
=======
            
            if(!item.CollidedObject.Contains("floor")){
                    
                HandleFeedback(item);
            }
            
            
            
            
>>>>>>> Stashed changes
        }
    }

    private GameObject LocateCollidedObjectRoot(GameObject collidedObject) {

        var feedbackSettings = collidedObject.GetComponent<ObjectFeedbackSettings>();
        GameObject currentObject = collidedObject;

        while (feedbackSettings == null) {
            // Get parent of the object we're looking at
            currentObject = currentObject.transform.parent.gameObject;
            feedbackSettings = currentObject.GetComponent<ObjectFeedbackSettings>();
        }

        return currentObject;
    }

    private void HandleCollisionEnter(Collision collision)
    {
        // Collisions with the Player game object are reported sometimes. This causes problems in the
        // LocateCollidedObjectRoot method, since the Player is located in the scene root (has no parent)
        if (collision.gameObject.name == "Player") return;

        // Since objects have their mesh colliders placed in the inner objects in the hierarchy,
        // we have to "move up" the object tree until we find the root object of the prop (which 
        // contains the FeedbackSettings component)
        GameObject collidedObject = LocateCollidedObjectRoot(collision.gameObject);

        string collidedObjectTag = GetObjectName(collidedObject);
        string playerColliderTag = GetObjectName(gameObject);

        var feedbackSettings = collidedObject.GetComponent<ObjectFeedbackSettings>()?.settings;
        
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
        GameObject collidedObject = LocateCollidedObjectRoot(collision.gameObject);

        string collidedObjectTag = GetObjectName(collidedObject);
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
        if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
        {
            if (!item.CanPlay)
            {
                // Debug.Log($"CanPlay: {item.CanPlay}, {item.CollidedObject}, {item.IsColliding}, {item.TimeColliding},{item.FeedbackSettings?.feedbackTypes?.FirstOrDefault()}, {item.FeedbackSettings?.sound?.name}");
                item.CanPlay = true;
            }
        }
        else
        {
            if (item.CanPlay)
            {
                // Debug.LogError($"CanPlay: {item.CanPlay}, {item.CollidedObject}, {item.IsColliding}, {item.TimeColliding},{item.FeedbackSettings?.feedbackTypes?.FirstOrDefault()}, {item.FeedbackSettings?.sound?.name}");
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
        var source = SoundSources.GetValueOrDefault(collision.CollidedObject);
        if (source is null)
        {
            source = gameObject.AddComponent<AudioSource>();
            SoundSources.Add(collision.CollidedObject, source);
        }

        if (collision.CanPlay && collision.IsColliding)
        {
            // Debug.Log($"aaaaaaaaa: {collision.CanPlay}, {collision.CollidedObject}, {collision.IsColliding}, {!source.isPlaying}");
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
            // Debug.LogError($"aaaaaaaaa: {collision.CanPlay}, {collision.CollidedObject}, {collision.IsColliding}");
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
