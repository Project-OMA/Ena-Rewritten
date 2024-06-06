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

    private string[] tagPrefab = {"floor", "wall", "Furniture", "Utensils", "Electronics", "Goals"};

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
        CreateDictSource();
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

    private void CreateDictSource(){
        AudioSource source;
        foreach (string tag in tagPrefab){

            source = gameObject.AddComponent<AudioSource>();
            source.loop = false;
            SoundSources.Add(tag, source);

        }
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
        if (collision.gameObject.name == "Player") return;

        // Since objects have their mesh colliders placed in the inner objects in the hierarchy,
        // we have to "move up" the object tree until we find the root object of the prop (which 
        // contains the FeedbackSettings component)
        GameObject collidedObject = LocateCollidedObjectRoot(collision.gameObject);

        string collidedObjectTag = GetObjectName(collidedObject);
        string playerColliderTag = GetObjectName(gameObject);

        var feedbackSettings = collidedObject.GetComponent<ObjectFeedbackSettings>()?.settings;

        if (collidedObject.tag == "floor") {

            
            Debug.Log("Caneeee");
            
            handleCaneCollision(collision);
            
            

        }else{
            if (Collisions.TryGetValue(collidedObjectTag + playerColliderTag, out var item))
            {
                item.IsColliding = true;
                HandleCollisionEnterFeedback(item);
            }
            else
            {
            var collisionEvent = new CollisionEvent(
                collidedObject: collidedObjectTag,
                collisionLocationOnPlayer: playerColliderTag,
                feedbackSettings: feedbackSettings);
            
                Collisions.Add(collidedObjectTag + playerColliderTag, collisionEvent);
                collisionEvent.IsColliding = true;
                HandleCollisionEnterFeedback(collisionEvent);
                
            }
        }

        
        
        
    }
    public void handleCaneCollision(Collision collision){


        GameObject collidedObject = LocateCollidedObjectRoot(collision.gameObject);
        var feedbackSettings = collidedObject.GetComponent<ObjectFeedbackSettings>()?.settings;
        
        Debug.Log(feedbackSettings.sound);

        CaneSource.clip = feedbackSettings.sound;
        CaneSource.playOnAwake = true;
        CaneSource.spatialBlend = 1.0f;
        CaneSource.spatialize = true;
        CaneSource.loop = false;

        if(!CaneSource.isPlaying){
            CaneSource.Play();
        }
        

        

    }

    private void HandleCollisionExit(Collision collision)
    {
        if (collision.gameObject.name == "Player") return;

        GameObject collidedObject = LocateCollidedObjectRoot(collision.gameObject);

        if (collidedObject.tag != "floor"){

            string collidedObjectTag = GetObjectName(collidedObject);
            string playerColliderTag = GetObjectName(gameObject);

            if (Collisions.TryGetValue(collidedObjectTag + playerColliderTag, out var itemToUpdate))
            {
                itemToUpdate.IsColliding = false;
                HandleCollisionExitFeedback(itemToUpdate);
            }

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
        Vector3 rayOffset = new Vector3(0,1,0);
        Vector3 origin = transform.position + rayOffset;
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

        int lineSeparatorIndex = inputString.IndexOf("%");


        string firstLine = lineSeparatorIndex >= 0 ? inputString.Substring(0, lineSeparatorIndex) : inputString;

        var source = SoundSources.GetValueOrDefault(firstLine);

        Debug.Log(firstLine);

        if(tagPrefab.Contains(firstLine)){
            
            if(firstLine == "floor"){
                source.spatialBlend = 1.0f;
                source.spatialize = true;
            }


            source.loop = false;
            source.clip = sound;

        }else{
            source = SoundSources.GetValueOrDefault(collision.CollidedObject);

            if (source is null)

                {   
                    source = gameObject.AddComponent<AudioSource>();
                    source.loop = false;
                    SoundSources.Add(collision.CollidedObject, source);
                    

                    
                    
                } else{
                    source.clip = sound;
                }

        }

        
        
        
        
        if (collision.IsColliding)
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

    private Vector3 checkpos(){

        var move = interactionController.getMoveVector();
        
        var playerpos = gameObject.transform.position;

        var potentialpos = move+playerpos;

        return potentialpos;

    }

    #endregion
}
