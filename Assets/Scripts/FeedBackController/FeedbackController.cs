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

    private string[] tagPrefab = {"floor"};

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
        List<InputDevice> devices = new List<InputDevice>();
        var isRightHand = gameObject.name.ToLower().Contains("right");
        var isLeftHand = gameObject.name.ToLower().Contains("left");
        InputDevices.GetDevicesAtXRNode(isRightHand ? XRNode.RightHand : XRNode.LeftHand, devices);
        inputDevice = devices.Count > 0 ? devices[0] : default;
    }

    private void InitializeFeedbackComponents()
    {
        HapticImpulse = new HapticFeedback(inputDevice, this);
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

    private void HandleCollisionFeedback()
    {
        
        foreach (var item in Collisions.Values)
        {
            
                HandleFeedback(item);
            
            
        }
    }
    

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
        
        
    }
    public void handleCaneCollision(Collision collision){


        GameObject collidedObject = LocateCollidedObjectRoot(collision.gameObject);
        var feedbackSettings = collidedObject.GetComponent<ObjectFeedbackSettings>()?.settings;
        
        Debug.Log(feedbackSettings.sound);

        CaneSource.clip = feedbackSettings.sound;
        CaneSource.Play();

        

    }

    private void HandleCollisionExit(Collision collision)
    {
        if (collision.gameObject.name == "Player") return;

        GameObject collidedObject = LocateCollidedObjectRoot(collision.gameObject);

        if (collidedObject.tag == "floor"){
            CaneSource.Stop();
        }

        

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

    public void handleWallCollision(bool toggle) {

        if(!toggle){
            WallStopSource.Play();
        }
       

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

        
        string inputString = collision.CollidedObject;

        int lineSeparatorIndex = inputString.IndexOf("%");


        string firstLine = lineSeparatorIndex >= 0 ? inputString.Substring(0, lineSeparatorIndex) : inputString;

        var source = SoundSources.GetValueOrDefault(collision.CollidedObject);

        if(firstLine == "floor"){
            source = SoundSources.GetValueOrDefault(firstLine);
            source.spatialBlend = 1;
            source.loop = false;
        }
        
        if (source is null)
        {   
            source = gameObject.AddComponent<AudioSource>();
            source.loop = false;
            source.spatialBlend = 1;
            SoundSources.Add(collision.CollidedObject, source);
            

            
            
        } else{
            source.clip = sound;
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
