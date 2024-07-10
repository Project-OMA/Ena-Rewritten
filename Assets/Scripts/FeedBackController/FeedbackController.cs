using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.XR;
using System.Linq;

public class FeedbackController : MonoBehaviour
{
    #region Fields

    private InputDevice leftHandDevice;
    private InputDevice rightHandDevice;
    private HapticFeedback HapticImpulseLeft;
    private HapticFeedback HapticImpulseRight;

    private static readonly Dictionary<string, CollisionEvent> Collisions = new Dictionary<string, CollisionEvent>();
    private static readonly Dictionary<string, CollisionEvent> FloorDetects = new Dictionary<string, CollisionEvent>();

    private readonly string fileName = $"{Directory.GetCurrentDirectory()}/PlayerLogs/feedback.csv";

    #endregion

    #region Properties

    public Dictionary<string, AudioSource> SoundSources { get; private set; }
    public Rigidbody Rb { get; set; }
    public InputDevice inputDevice;
    private HapticFeedback HapticImpulse;

    public GameObject cane;

    private AudioSource WallStopSource;
    private AudioSource CaneSource;

    public AudioSource WalkSource;

    public Transform cam;
    

    private InteractionController interactionController;

    
    


    #endregion

    #region Unity Callbacks

    void Start()
    {
        //Fetch the AudioSource from the GameObject
        WallStopSource = gameObject.AddComponent<AudioSource>();
        CaneSource = gameObject.AddComponent<AudioSource>();
        interactionController = GetComponent<InteractionController>();
        

        AudioClip audioClip = Resources.Load<AudioClip>("Sounds/alarme");
        
        WallStopSource.clip = audioClip;
        
        
    }
    private void Awake()
    {
        InitializeInputDevice();
        InitializeFeedbackComponents();
        
    }

    private void Update()
    {
        AudioCleaner();
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
        if (collision.gameObject.tag == "Player") return;

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
        if (collision.gameObject.tag == "Player") return;

        GameObject collidedObject = LocateCollidedObjectRoot(collision.gameObject);

        string collidedObjectTag = GetObjectName(collidedObject);
        string playerColliderTag = GetObjectName(gameObject);

        if (Collisions.TryGetValue(collidedObjectTag + playerColliderTag, out var itemToUpdate))
        {
            itemToUpdate.IsColliding = false;
            HandleCollisionExitFeedback(itemToUpdate);
        }

        
    }

    #endregion

    #region Feedback Handling

    public void handleWallCollision(bool toggle) {

        if(!toggle){
            WallStopSource.Play();
        }
       

    }

    public void handleStep() {
        Debug.Log("Player did a step");
        DetectFloor();
    }
    
    private void DetectFloor()
    {
        float raylen = 0.5f;
        Vector3 rayOffset = new Vector3(0,-0.5f,0);
        Vector3 origin = cam.transform.position + rayOffset;
        Vector3 direction = transform.up;

        RaycastHit hit;
        

    


        CollisionEvent collisionEvent=null;
        Debug.DrawRay(origin, -direction, Color.green, 2, true);



        if (Physics.Raycast(origin, -direction, out hit, 2.0f))
            
            
            
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
                            collisionEvent.Vector3 = hit.point;
                            HandleWalkFeedback(item);
                            
                        }
                        else
                        {
                            
                            collisionEvent = new CollisionEvent(
                                collidedObject: collidedObjectTag,
                                collisionLocationOnPlayer: playerColliderTag,
                                feedbackSettings: feedbackSettings,
                                gameObject: collidedObject,
                                vector3: hit.point);
                            
                            collisionEvent.IsColliding = true;
                            collisionEvent.IsRay = true;
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
        

        

        if (collision.IsColliding && collision.CanPlay)
            {
            // Debug.Log($"aaaaaaaaa: {collision.CanPlay}, {collision.CollidedObject}, {collision.IsColliding}, {!source.isPlaying}");
                if (collision.IsRay){
                    if(!WalkSource.isPlaying){
                        WalkSource.clip = sound;
                        WalkSource.Play();
                    }
                }else{

                    Transform parentObj = collision.GameObject.transform;
                    Transform spSound = parentObj.Find("SpatialSound");

                    if(spSound == null){

                        
                        GameObject audioObject = new GameObject("SpatialSound");
                        audioObject.transform.position = collision.Vector3;
                        audioObject.transform.parent = collision.GameObject.transform;

                        Debug.Log("CHILD: "+ collision.GameObject.transform.childCount);
                        AudioSource audioSource = audioObject.AddComponent<AudioSource>();
                        audioSource.spatialBlend = 1.0f;
                        

                        if(!audioSource.isPlaying){
                            audioSource.clip = sound;
                            audioSource.Play();
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
            if (!HapticImpulseLeft.isPlaying && !HapticImpulseRight.isPlaying)
            {
                HapticImpulseLeft.Play(hapticForce);
                HapticImpulseRight.Play(hapticForce);
            }
        }
        else
        {
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

    private void AudioCleaner(){

        foreach(var col in Collisions){

            CollisionEvent item = col.Value;
            GameObject gameObjectitem = item.GameObject.gameObject;
            AudioSource source = gameObjectitem.GetComponent<AudioSource>();
            
            if(!item.IsColliding){
                if(source && !source.isPlaying){
                  Destroy(gameObjectitem.GetComponent<AudioSource>());  
                }
                
            }
        }
    }

    private Vector3 checkpos(){

        var move = interactionController.getMoveVector();
        
        var playerpos = gameObject.transform.position;

        var potentialpos = move+playerpos;

        return potentialpos;

    }

    #endregion
}
