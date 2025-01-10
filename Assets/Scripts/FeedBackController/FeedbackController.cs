using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.XR;
using System.Linq;

public class FeedbackController : MonoBehaviour
{
    #region Fields

 
    private static readonly Dictionary<string, CollisionEvent> FloorDetects = new Dictionary<string, CollisionEvent>();
    private static readonly Dictionary<string, CollisionEvent> ObjDetects = new Dictionary<string, CollisionEvent>();

    #endregion

    #region Properties

    public Dictionary<string, AudioSource> SoundSources { get; private set; }

    public AudioSource WalkSource;

    public GameObject ObjSource;

    public Transform cam;

    private string map;


    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        InitializeFeedbackComponents();
        
    }

    private void Update()
    {
        //HandleCollisionFeedback();
        //DetectFloor();
    }

    private void Start(){


        ObjSource = GameObject.Find("ObjSource");


    }

    #endregion

    #region Initialization

    

    private void InitializeFeedbackComponents()
    {
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


    #endregion

    #region Feedback Handling



    public void handleStep() {
        Debug.Log("Player did a step");
        DetectFloor();
    }


    public void ObjectDectetorForCollision(GameObject collidedObject, Vector3 point){
        

        if (collidedObject.tag == "Player" || collidedObject.tag == "Cane" || collidedObject.tag == "Left" || collidedObject.tag == "car") return;
        
        Debug.Log("AAAAAAAAAAAAAA"+collidedObject);

        collidedObject = LocateCollidedObjectRoot(collidedObject);

        string collidedObjectTag = GetObjectName(collidedObject);
        string playerColliderTag = GetObjectName(gameObject);

        CollisionEvent collisionEvent=null;

        var feedbackSettings = collidedObject.GetComponent<ObjectFeedbackSettings>()?.settings;

            if (ObjDetects.TryGetValue(collidedObjectTag + playerColliderTag, out var item))
            {
                
                collisionEvent = item;
                collisionEvent.IsColliding = true;
                collisionEvent.Vector3 = point;
                HandleFeedback(item);
                
            }
            else
            {
                if(MapLoader.hasMenu){
                    map = MapLoader.mapMenu;
                }else{
                    map = MapLoader.mapNoMenu;
                }
                
                collisionEvent = new CollisionEvent(
                    collidedObject: collidedObjectTag,
                    whatcollided: playerColliderTag,
                    feedbackSettings: feedbackSettings,
                    gameObject: collidedObject,
                    vector3: point,
                    currentMap: map,
                    totalCollisions:0);
                
                collisionEvent.IsColliding = true;
                collisionEvent.IsRay = true;
                ObjDetects.Add(collidedObjectTag + playerColliderTag, collisionEvent);
                HandleFeedback(collisionEvent);
                
            }
        

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
                            if(MapLoader.hasMenu){
                                map = MapLoader.mapMenu;
                            }else{
                                map = MapLoader.mapNoMenu;
                            }
                            
                            collisionEvent = new CollisionEvent(
                                collidedObject: collidedObjectTag,
                                whatcollided: playerColliderTag,
                                feedbackSettings: feedbackSettings,
                                gameObject: collidedObject,
                                vector3: hit.point,
                                currentMap: map,
                                totalCollisions:0);
                            
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
    

    private void HandleFeedback(CollisionEvent collision)
    {
        foreach (var feedbackType in collision.FeedbackSettings?.feedbackTypes ?? new FeedbackTypeEnum[0])
        {
            switch (feedbackType)
            {
                case FeedbackTypeEnum.Sound1:
                    PlaySoundFeedback(collision.FeedbackSettings.sound1, collision);
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
                if (collision.GameObject.tag=="floor"){
                    if(!WalkSource.isPlaying){
                        WalkSource.clip = sound;
                        WalkSource.Play();
                    }
                }else{
                    ObjSource.transform.position = collision.Vector3;
                    ObjSource.transform.parent = collision.GameObject.transform;
                    AudioSource audioSource = ObjSource.GetComponent<AudioSource>();

                    if(!audioSource.isPlaying){
                        audioSource.clip = sound;
                        audioSource.Play();
                }
                }
            
        }

    }

    #endregion

    #region Utility Methods

    public string GetObjectName(GameObject gameObject)
    {
        return string.IsNullOrEmpty(gameObject.tag) ? gameObject.name : gameObject.tag + "%" + gameObject.name;
    }


    #endregion
}
