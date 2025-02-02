using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.XR;
using System.Linq;
using System.Collections;

public class HandFeedback : MonoBehaviour
{
    #region Fields

    private InputDevice leftHandDevice;
    private InputDevice rightHandDevice;
    private HapticFeedback HapticImpulseLeft;
    private HapticFeedback HapticImpulseRight;

    public Transform rightController;
    public Transform leftController;


    

    public static readonly Dictionary<string, CollisionEvent> Collisions = new Dictionary<string, CollisionEvent>();
    

    private readonly string fileName = $"{Directory.GetCurrentDirectory()}/PlayerLogs/feedback.csv";

    #endregion

    #region Properties

    public Dictionary<string, AudioSource> SoundSources { get; private set; }
    public GameObject CaneSource;
    public GameObject WallSource;

    private Transform soundChild;
    private string map;
    private bool noSoundChild = true;

    public AudioSource audioTrail;

    private bool innerFeedbackLeft = false;
    private bool innerFeedbackRight = false;
    private Transform currentController;

    public Transform rayPos;

    private float nextUpdate=0.2f;

    
    


    #endregion

    #region Unity Callbacks

    void Start()
    {
        
        audioTrail = GameObject.Find("AudioTrailSource").GetComponent<AudioSource>();
        


        
        
    }
    private void Awake()
    {
        InitializeInputDevice();
        InitializeFeedbackComponents();
        
    }

    private void Update()
    {

        //meme big boy

       
    }

    private void OnCollisionEnter(Collision collision)
    {
        HandleCollisionEnter(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.tag != "floor"){
            StartCoroutine(FeedbackRoutine());
        }
        

        HandleCollisionExit(collision);
        
        
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

        
        CaneSource = GameObject.Find("CaneSource");
        WallSource = GameObject.Find("WallSource");
        
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
        if (collision.gameObject.tag == "car") return;

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
            if(item.Whatcollided== "Cane Right Controller" || item.Whatcollided == "Left Left Controller"){


                if(HandCheck.LeftHand){
            
                    innerFeedbackLeft = true;
                }

                if(HandCheck.RightHand){
            
                    innerFeedbackRight = true;
                }

            }   

            if(!item.IsColliding && !item.CanPlay){
                item.IsColliding = true;
                item.Vector3 = contact.point;
                item.GameObject = collidedObject;
                noSoundChild = true;


                for (int i = 0; i < item.GameObject.transform.childCount; i++) {
                    var soundChild = item.GameObject.transform.GetChild(i);
                    
                    if (soundChild.CompareTag("SoundTag")) {
                        Debug.Log("hii");
                        
                        var audio = soundChild.GetComponent<AudioSource>();
                        if (!audio.isPlaying) {
                            item.TotalCollisions += 1;
                            noSoundChild = false;
                            HandleCollisionEnterFeedback(item);
                        }
                        
                        break; 

                    } 
                    
                }

                if(noSoundChild){
                    item.TotalCollisions += 1;
                    HandleCollisionEnterFeedback(item);
                }     
                
            }

            
        }
        else
        {
            if(MapLoader.hasMenu){
                map = MapLoader.mapMenu;
            }else{
                map = MapLoader.mapNoMenu;
            }


        var collisionEvent = new CollisionEvent(
            collidedObject: collidedObjectTag,
            whatcollided: playerColliderTag,
            feedbackSettings: feedbackSettings,
            gameObject: collidedObject,
            vector3: contact.point,
            totalCollisions: 1,
            currentMap: map);
        
            Collisions.Add(collidedObjectTag + playerColliderTag, collisionEvent);
            collisionEvent.IsColliding = true;

            if(collisionEvent.Whatcollided== "Cane Right Controller" || collisionEvent.Whatcollided == "Left Left Controller"){


                if(HandCheck.LeftHand){
            
                    innerFeedbackLeft = true;
                }

                if(HandCheck.RightHand){
            
                    innerFeedbackRight = true;
                }

            }  
            
            HandleCollisionEnterFeedback(collisionEvent);
            
        }
        
        
    }
    

    private void HandleCollisionExit(Collision collision)
    {

        Debug.Log("amberlamps"+ collision.gameObject.tag);
        
        

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
        Debug.Log("mekme ");

        

        
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
        Debug.Log("ColOver");
        item.CanPlay = false;
        HandleFeedback(item);
        
    }


    

   
   

        
    
    

    private void HandleFeedback(CollisionEvent collision)
    {
        foreach (var feedbackType in collision.FeedbackSettings?.feedbackTypes ?? new FeedbackTypeEnum[0])
        {
            
            if(collision.GameObject.tag!="floor"){
                switch(collision.TotalCollisions){

                    case 1:
                        PlaySoundFeedback(collision.FeedbackSettings.sound1, collision);
                        PlayHapticFeedback(collision.FeedbackSettings.hapticForce, collision);
                        break;
                    
                    case 2:
                        PlaySoundFeedback(collision.FeedbackSettings.sound2, collision);
                        PlayHapticFeedback(collision.FeedbackSettings.hapticForce, collision);
                        break;
                    
                    case 3:

                        PlaySoundFeedback(collision.FeedbackSettings.sound1, collision);
                        PlayHapticFeedback(collision.FeedbackSettings.hapticForce, collision);


                        audioTrail.clip = collision.FeedbackSettings.sound3;
                        audioTrail.Play();
                        
                        

                        collision.TotalCollisions = 0;
                        
                        break;
                    
                }

                }else{
                    PlaySoundFeedback(collision.FeedbackSettings.sound2, collision);
                }


                
                    
            }
        }
    

    #endregion

    #region Feedback Methods



    private void PlaySoundFeedback(AudioClip sound, CollisionEvent collision)
    {

        
        string inputString = collision.CollidedObject;

        Debug.Log(collision.GameObject);

        int lineSeparatorIndex = inputString.IndexOf(" ");


        string firstLine = lineSeparatorIndex >= 0 ? inputString.Substring(0, lineSeparatorIndex) : inputString;

        

        Debug.Log("Tag:" + firstLine);
        Debug.Log("Name: " + inputString);
        

        

        if (collision.IsColliding && collision.CanPlay)
            {
            // Debug.Log($"aaaaaaaaa: {collision.CanPlay}, {collision.CollidedObject}, {collision.IsColliding}, {!source.isPlaying}");
                

                    Transform parentObj = collision.GameObject.transform;
                    Transform spSound = parentObj.Find("SpatialSound");

                    if(spSound == null){

                        if (collision.GameObject.tag=="wall"){


                            WallSource.transform.position = collision.Vector3;
                            WallSource.transform.parent = collision.GameObject.transform;
                            AudioSource audioSource = WallSource.GetComponent<AudioSource>();

                            if(!audioSource.isPlaying){
                                audioSource.clip = sound;
                                audioSource.Play();

                            } 

                        }else if(collision.GameObject.tag=="floor"){

                            CaneSource.transform.position = collision.Vector3;
                            CaneSource.transform.parent = collision.GameObject.transform;
                            AudioSource audioSource = CaneSource.GetComponent<AudioSource>();

                            if(!audioSource.isPlaying){
                                audioSource.clip = sound;
                                audioSource.Play();
                            } 

                        }else{

                            GameObject audioObject = new GameObject("SpatialSound");
                            audioObject.tag = "SoundTag";
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
        if (collision.IsColliding)
        {

                if(HandCheck.LeftHand){
                    HapticImpulseLeft.Play(hapticForce);
                   
                }

                if(HandCheck.RightHand){
                    HapticImpulseRight.Play(hapticForce);   
                    
                }
                
                
        }
    }

    public void TrafficHaptic(){

        
        HapticImpulseLeft.Play(0.5f);
            
        HapticImpulseRight.Play(0.5f);   
                      

    }

    public void stopHaptic(){

        HapticImpulseLeft.Stop();
            
        HapticImpulseRight.Stop(); 

    }

    #endregion

    #region Utility Methods

    public string GetObjectName(GameObject gameObject)
    {
        return string.IsNullOrEmpty(gameObject.tag) ? gameObject.name : gameObject.tag + " " + gameObject.name;
    }

    public void DetectController(Transform currentController, string tag)
    {
        RaycastHit hit;
        Vector3 direction = (currentController.position - rayPos.position).normalized;
        Debug.Log("memebigboy");
        
        Debug.DrawRay(rayPos.position, direction * 5, Color.red, 5, true);

        if (Physics.Raycast(rayPos.position, direction, out hit, 5))
        {
            if (hit.collider != null && hit.collider.gameObject.tag == tag)
            {
                Debug.Log("Detected object with tag: " + tag);

                if (tag == "Left")
                {
                    Debug.Log("Found");
                    HandCheck.LeftHand = false;
                    innerFeedbackLeft = false;
                    HapticImpulseLeft.Stop();
                }
                else if (tag == "Cane")
                {
                    Debug.Log("Found");
                    HandCheck.RightHand = false;
                    innerFeedbackRight = false;
                    HapticImpulseRight.Stop();
                }
            }
            
        }
}

private IEnumerator FeedbackRoutine()
{


    while (innerFeedbackLeft || innerFeedbackRight)
    {
        yield return new WaitForSeconds(0.15f);


        if (innerFeedbackLeft)
        {
            Debug.Log("HALLO :D");
            Debug.Log(leftController.position);
            DetectController(leftController, "Left");
        }

        if (innerFeedbackRight)
        {
            Debug.Log("HALLO :D");
            rightController = GameObject.Find("collisionRight").transform;
            Debug.Log("HEYYY"+rightController.position);
            DetectController(rightController, "Cane");
        }

        
    }
}


    #endregion
}
