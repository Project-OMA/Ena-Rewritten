using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.XR;
using System.Linq;

public class HandFeedback : MonoBehaviour
{
    #region Fields

    private InputDevice leftHandDevice;
    private InputDevice rightHandDevice;
    private HapticFeedback HapticImpulseLeft;
    private HapticFeedback HapticImpulseRight;

    private static readonly Dictionary<string, CollisionEvent> Collisions = new Dictionary<string, CollisionEvent>();

    private readonly string fileName = $"{Directory.GetCurrentDirectory()}/PlayerLogs/feedback.csv";

    #endregion

    #region Properties

    public Dictionary<string, AudioSource> SoundSources { get; private set; }
    public GameObject CaneSource;
    public GameObject WallSource;



    
    


    #endregion

    #region Unity Callbacks

    void Start()
    {
        //Fetch the AudioSource from the GameObject
        
        
    }
    private void Awake()
    {
        InitializeInputDevice();
        InitializeFeedbackComponents();
        
    }

    private void Update()
    {
        //HandleCollisionFeedback();
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
        List<InputDevice> leftHandDevices = new List<InputDevice>();
        List<InputDevice> rightHandDevices = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, leftHandDevices);
        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, rightHandDevices);
        leftHandDevice = leftHandDevices.Count > 0 ? leftHandDevices[0] : default;
        rightHandDevice = rightHandDevices.Count > 0 ? rightHandDevices[0] : default;

    }

    private void InitializeFeedbackComponents()
    {
        HapticImpulseLeft = new HapticFeedback(leftHandDevice, this);
        HapticImpulseRight = new HapticFeedback(rightHandDevice, this);
        SoundSources = new Dictionary<string, AudioSource>();
    }

    #endregion

    #region Collision Handling
    

    public GameObject LocateCollidedObjectRoot(GameObject collidedObject) {

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
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Cane" || collision.gameObject.tag == "Left") return;

        ContactPoint contact = collision.contacts[0];
        Debug.Log("Pos:" + contact.point);

        // Since objects have their mesh colliders placed in the inner objects in the hierarchy,
        // we have to "move up" the object tree until we find the root object of the prop (which 
        // contains the FeedbackSettings component)
        GameObject collidedObject = LocateCollidedObjectRoot(collision.gameObject);

        string collidedObjectTag = GetObjectName(collidedObject);
        string playerColliderTag = GetObjectName(gameObject);

        var feedbackSettings = collidedObject.GetComponent<ObjectFeedbackSettings>()?.settings;

        TutorialCheckpoints.playerHasInteracted = true;

       
        if (Collisions.TryGetValue(collidedObjectTag + playerColliderTag, out var item))
        {
            item.IsColliding = true;
            item.Vector3 = contact.point;
            HandleCollisionEnterFeedback(item);
        }
        else
        {
        var collisionEvent = new CollisionEvent(
            collidedObject: collidedObjectTag,
            collisionLocationOnPlayer: playerColliderTag,
            feedbackSettings: feedbackSettings,
            gameObject: collidedObject,
            vector3: contact.point);
        
            Collisions.Add(collidedObjectTag + playerColliderTag, collisionEvent);
            collisionEvent.IsColliding = true;
            HandleCollisionEnterFeedback(collisionEvent);
            
        }
        

        
        
        
    }
    

    private void HandleCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Cane" || collision.gameObject.tag == "Left") return;

        GameObject collidedObject = LocateCollidedObjectRoot(collision.gameObject);


        string collidedObjectTag = GetObjectName(collidedObject);
        string playerColliderTag = GetObjectName(gameObject);

        if (collision.gameObject.tag == "DoorWindow"){

            TutorialCheckpoints.playerDoor = true;
        }

        if (Collisions.TryGetValue(collidedObjectTag + playerColliderTag, out var itemToUpdate))
        {
            itemToUpdate.IsColliding = false;
            HandleCollisionExitFeedback(itemToUpdate);
        }

        
    }

    #endregion

    #region Feedback Handling

    


    private void HandleCollisionEnterFeedback(CollisionEvent item)
    {
        
        item.CanPlay = true;
        HandleFeedback(item);
            
    }

    private void HandleCollisionExitFeedback(CollisionEvent item)
    {
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
                    PlayHapticFeedback(collision.FeedbackSettings.hapticForce, collision);
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

        
        string inputString = collision.CollidedObject;

        Debug.Log(collision.GameObject);

        int lineSeparatorIndex = inputString.IndexOf("%");


        string firstLine = lineSeparatorIndex >= 0 ? inputString.Substring(0, lineSeparatorIndex) : inputString;

        

        Debug.Log("Tag:" + firstLine);
        Debug.Log("Name: " + inputString);

        // wip: continous sound
        void RewindSound(AudioSource audioSource, AudioClip sound)
        {
            float newTime = audioSource.time - sound.length * 0.25f;
            audioSource.time = Mathf.Max(newTime, 0);
        }
        

        

        if (collision.IsColliding && collision.CanPlay)
            {
            // Debug.Log($"aaaaaaaaa: {collision.CanPlay}, {collision.CollidedObject}, {collision.IsColliding}, {!source.isPlaying}");
                

                    Transform parentObj = collision.GameObject.transform;
                    Transform spSound = parentObj.Find("SpatialSound");

                    if(spSound == null){

                        if (collision.GameObject.tag=="wall"){


                            WallSource.transform.position = collision.Vector3;
                            AudioSource audioSource = WallSource.GetComponent<AudioSource>();

                            if(!audioSource.isPlaying){
                                audioSource.clip = sound;
                                audioSource.Play();
                            } else {
                                RewindSound(audioSource, sound);
                            }

                        }else if(collision.GameObject.tag=="floor"){

                            CaneSource.transform.position = collision.Vector3;
                            AudioSource audioSource = CaneSource.GetComponent<AudioSource>();

                            if(!audioSource.isPlaying){
                                audioSource.clip = sound;
                                audioSource.Play();
                            } else {
                                RewindSound(audioSource, sound);
                            }

                        }else{

                            GameObject audioObject = new GameObject("SpatialSound");
                            audioObject.transform.position = collision.Vector3;
                            audioObject.transform.parent = collision.GameObject.transform;

                            Debug.Log("CHILD: "+ collision.GameObject.transform.childCount);
                            AudioSource audioSource = audioObject.AddComponent<AudioSource>();
                            audioSource.spatialBlend = 1.0f;
                            

                            if(!audioSource.isPlaying){
                                audioSource.clip = sound;
                                audioSource.Play();
                            } else {
                                RewindSound(audioSource, sound);
                            }
                            
                            Destroy(audioObject, sound.length);

                            }
                    

                }
            
        }

        
    
    }

    private void PlayHapticFeedback(float hapticForce, CollisionEvent collision)
    {
        if (collision.CanPlay && collision.IsColliding)
        {

                if(HandCheck.LeftHand){
                    HapticImpulseLeft.Play(hapticForce);
                }

                if(HandCheck.RightHand){
                    HapticImpulseRight.Play(hapticForce);   
                }
                
                
        }
        else
        {
            HandCheck.LeftHand = false;
            HandCheck.RightHand = false;
            HapticImpulseLeft.Stop();
            HapticImpulseRight.Stop();
        }
    }

    #endregion

    #region Utility Methods

    public string GetObjectName(GameObject gameObject)
    {
        return string.IsNullOrEmpty(gameObject.tag) ? gameObject.name : gameObject.tag + "%" + gameObject.name;
    }

    private void SaveCollisionDataToCsv()
    {
        CsvWriter.WriteToCsv(fileName, Collisions.Values);
    }

    #endregion
}
