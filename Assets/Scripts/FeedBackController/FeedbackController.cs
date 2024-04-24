using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.XR;
using System.Linq;

public class FeedbackController : MonoBehaviour
{
    #region Fields

    private static readonly Dictionary<string, CollisionEvent> Collisions = new Dictionary<string, CollisionEvent>();
    private static readonly Dictionary<string, CollisionEvent> FloorDetects = new Dictionary<string, CollisionEvent>();

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
        //DetectFloor();
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
            
                HandleFeedback(item);
            
            
        }
    }
    

    private GameObject LocateCollidedObjectRoot(GameObject collidedObject) {

        var feedbackSettings = collidedObject.GetComponent<ObjectFeedbackSettings>();
        GameObject currentObject = collidedObject;

        while (feedbackSettings == null) {
            Debug.Log(collidedObject.name);
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
        if (collision.gameObject.name == "Player") return;

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

    public void handleWallCollision(GameObject collidedObject) {
        Debug.Log("Player collided with wall " + collidedObject.name);
    }

    public void handleStep() {
        Debug.Log("Player did a step");
        DetectFloor();
    }

    public void handleMovementStart() {

        //DetectFloor();

        //HandleWalkFeedback();
    }

    public void handleMovementStop() {

        //DetectFloor();

        //HandleStopFeedback();
    }
    
    private void DetectFloor()
    {
        RaycastHit hit;
        CollisionEvent collisionEvent=null;
    
        Debug.Log("RAYCAST");

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 2))
            {
            if (hit.collider != null) {
                    if (hit.collider.gameObject.tag == "floor") {

                        GameObject collidedObject = hit.collider.gameObject;

                        string collidedObjectTag = GetObjectName(collidedObject);
                        string playerColliderTag = GetObjectName(gameObject);

                        var feedbackSettings = collidedObject.GetComponent<ObjectFeedbackSettings>()?.settings;

                        if (FloorDetects.TryGetValue(collidedObjectTag + playerColliderTag, out var item))
                        {
                            collisionEvent = item;
                            collisionEvent.IsColliding = true;
                            HandleWalkFeedback(item);
                            
                        }
                        else
                        {
                            collisionEvent = new CollisionEvent(
                                collidedObject: collidedObjectTag,
                                collisionLocationOnPlayer: playerColliderTag,
                                feedbackSettings: feedbackSettings);
                            
                            collisionEvent.IsColliding = true;
                            FloorDetects.Add(collidedObjectTag + playerColliderTag, collisionEvent);
                            HandleWalkFeedback(collisionEvent);
                            
                        }
                    }
                }
        } 
    }
    private void HandleWalkFeedback(CollisionEvent item) {
        item.CanPlay = true;
        HandleFeedback(item);
    }
    private void HandleStopFeedback(CollisionEvent item) {
        item.CanPlay = false;
        HandleFeedback(item);
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
            source.loop = false;
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
