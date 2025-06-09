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

    public static bool playerColliding = false;

    public int LeftAdder;
    public float HapticLeft;

    public List<float> hapticListLeft;

    public int RightAdder;
    public float HapticRight;
    public List<float> hapticListRight;

    public float ForceCutLeft;

    public float ForceCutRight;




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

    private float nextUpdate = 0.2f;





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


        if (collision.gameObject.tag != "floor")
        {
            StartCoroutine(VibrationVariation(collision));
        }
        

        
    }

    private void OnCollisionExit(Collision collision)
    {
       
        HandleCollisionExit(collision);

        StartCoroutine(FeedbackRoutine());


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


    public GameObject LocateCollidedObjectRoot(GameObject collidedObject)
    {

        var feedbackSettings = collidedObject.GetComponent<ObjectFeedbackSettings>();
        GameObject currentObject = collidedObject;

        while (feedbackSettings == null)
        {
            Debug.Log(collidedObject.name);
            // Get parent of the object we're looking at
            currentObject = currentObject.transform.parent.gameObject;
            Debug.Log("error object" + currentObject.name);
            feedbackSettings = currentObject.GetComponent<ObjectFeedbackSettings>();
        }

        return currentObject;
    }

    private void HandleCollisionEnter(Collision collision)
    {

        // Collisions with the Player game object are reported sometimes. This causes problems in the
        // LocateCollidedObjectRoot method, since the Player is located in the scene root (has no parent)
        if (collision.gameObject.tag == "car" && collision.gameObject.tag == "Player") return;

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
            if (item.Whatcollided == "Cane Right Controller" || item.Whatcollided == "Left Left Controller")
            {

                playerColliding = true;


                if (HandCheck.LeftHand)
                {

                    hapticListLeft = feedbackSettings.hapticValues;

                    innerFeedbackLeft = true;
                }

                if (HandCheck.RightHand)
                {


                    hapticListRight = feedbackSettings.hapticValues;

                    innerFeedbackRight = true;
                }

            }

            Debug.Log("BRUHV" + item.IsColliding);
            Debug.Log("BRUHV" + item.CanPlay);

            if (!item.CanPlay)
            {


                item.Vector3 = contact.point;
                item.GameObject = collidedObject;
                noSoundChild = true;


                for (int i = 0; i < item.GameObject.transform.childCount; i++)
                {
                    var soundChild = item.GameObject.transform.GetChild(i);

                    if (soundChild.CompareTag("SoundTag"))
                    {
                        Debug.Log("hii");

                        var audio = soundChild.GetComponent<AudioSource>();
                        if (!audio.isPlaying)
                        {
                            noSoundChild = false;
                            HandleCollisionEnterFeedback(item);
                        }

                        break;

                    }

                }

                if (noSoundChild)
                {
                    Debug.Log("MEMEBIGBOY");
                    item.TotalCollisions += 1;
                    HandleCollisionEnterFeedback(item);
                }

            }


        }
        else
        {

            if (MapLoader.hasMenu)
            {
                map = MapLoader.mapMenu;
            }
            else
            {
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


            if (collisionEvent.Whatcollided == "Cane Right Controller" || collisionEvent.Whatcollided == "Left Left Controller")
            {

                playerColliding = true;


                if (HandCheck.LeftHand)
                {

                    innerFeedbackLeft = true;
                }

                if (HandCheck.RightHand)
                {
                    innerFeedbackRight = true;
                }

            }

            HandleCollisionEnterFeedback(collisionEvent);

        }


    }


    private void HandleCollisionExit(Collision collision)
    {

        Debug.Log("amberlamps" + collision.gameObject.tag);



        GameObject collidedObject = LocateCollidedObjectRoot(collision.gameObject);


        string collidedObjectTag = GetObjectName(collidedObject);
        string playerColliderTag = GetObjectName(gameObject);

        if (collision.gameObject.tag == "DoorWindow")
        {

            TutorialCheckpoints.playerDoor = true;
        }

        if (Collisions.TryGetValue(collidedObjectTag + playerColliderTag, out var itemToUpdate))
        {

            

            Debug.Log("BRUHP" + itemToUpdate.IsColliding);
            if (collision.gameObject.tag == "floor")
            {

                playerColliding = false;
            }

            HandleCollisionExitFeedback(itemToUpdate);
        }


    }

    #endregion

    #region Feedback Handling




    private void HandleCollisionEnterFeedback(CollisionEvent item)
    {

        item.IsColliding = true;
        item.CanPlay = true;
        HandleFeedback(item);

    }

    private void HandleCollisionExitFeedback(CollisionEvent item)
    {
        Debug.Log("ColOver");
        item.IsColliding = false;
        item.CanPlay = false;

    }


    private void HandleFeedback(CollisionEvent collision)
    {
        foreach (var feedbackType in collision.FeedbackSettings?.feedbackTypes ?? new FeedbackTypeEnum[0])
        {

            if (collision.GameObject.tag != "floor")
            {
                switch (collision.TotalCollisions)
                {

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

            }
            else
            {
                FloorFeedback(collision);
            }




        }
    }

    private void FloorFeedback(CollisionEvent collision)
    {

        float floorhaptic = 0.0f;

        switch (collision.FeedbackSettings.materialtype)
        {

            case "Hard":

                floorhaptic = 0.5f;

                break;

            case "Soft":

                floorhaptic = 0.15f;

                break;

            case "Wet":

                floorhaptic = 0.3f;

                break;

        }



        PlaySoundFeedback(collision.FeedbackSettings.sound2, collision);
        PlayHapticFeedback(floorhaptic, collision);



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




        if (collision.CanPlay)
        {


            Transform parentObj = collision.GameObject.transform;
            Transform spSound = parentObj.Find("SpatialSound");

            if (spSound == null)
            {

                if (collision.GameObject.tag == "wall")
                {


                    WallSource.transform.position = collision.Vector3;
                    WallSource.transform.parent = collision.GameObject.transform;
                    AudioSource audioSource = WallSource.GetComponent<AudioSource>();

                    if (!audioSource.isPlaying)
                    {
                        audioSource.clip = sound;
                        audioSource.Play();

                    }

                }
                else if (collision.GameObject.tag == "floor")
                {

                    

                    CaneSource.transform.position = collision.Vector3;
                    CaneSource.transform.parent = collision.GameObject.transform;
                    AudioSource audioSource = CaneSource.GetComponent<AudioSource>();

                    if (!audioSource.isPlaying)
                    {
                        audioSource.clip = sound;
                        audioSource.Play();
                    }
                    else
                    {
                        audioSource.Stop();
                        audioSource.clip = sound;
                        audioSource.Play();
                    }

                }
                else
                {

                    GameObject audioObject = new GameObject("SpatialSound");
                    audioObject.tag = "SoundTag";
                    audioObject.transform.position = collision.Vector3;
                    audioObject.transform.parent = collision.GameObject.transform;

                    Debug.Log("CHILD: " + collision.GameObject.transform.childCount);
                    AudioSource audioSource = audioObject.AddComponent<AudioSource>();
                    audioSource.spatialBlend = 1.0f;


                    if (!audioSource.isPlaying)
                    {
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

        Debug.Log("JAAHAHAHAHAHA");
        if (collision.IsColliding)
        {

            if (HandCheck.LeftHand)
            {

                HapticLeft = hapticForce;

                HapticImpulseLeft.Play(hapticForce);

            }

            if (HandCheck.RightHand)
            {

                HapticRight = hapticForce;

                HapticImpulseRight.Play(hapticForce);

            }


        }
    }

    public void TrafficHaptic()
    {


        HapticImpulseLeft.Play(0.5f);

        HapticImpulseRight.Play(0.5f);


    }

    public void stopHaptic()
    {

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

                if (tag == "Left" && !HandCheck.LeftHand)
                {
                    LeftAdder = 0;
                    HapticLeft = 0.0f;
                    Debug.Log("Found");
                    playerColliding = false;
                    innerFeedbackLeft = false;
                    HapticImpulseLeft.Stop();
                }
                else if (tag == "Cane" && !HandCheck.RightHand)
                {
                    RightAdder = 0;
                    HapticRight = 0.0f;
                    playerColliding = false;
                    Debug.Log("Found" + rayPos.position);
                    innerFeedbackRight = false;
                    HapticImpulseRight.Stop();
                }
            }
        }
    }

    private IEnumerator FeedbackRoutine()
    {


        while ((innerFeedbackLeft || innerFeedbackRight))
        {

            Debug.Log("TESTME");
            yield return new WaitForSeconds(0.15f);



            if (innerFeedbackLeft)
            {

                if (LeftAdder % 10 == 0 && HapticLeft < 1.0f - ForceCutLeft && hapticListLeft.Count != 0)
                {

                    if (hapticListLeft.Count == LeftAdder / 10)
                    {
                        LeftAdder = 0;
                    }


                    float DifferenceLeft = hapticListLeft[LeftAdder / 10] - HapticLeft;

                    Debug.Log("memebughaptic" + HapticLeft);
                    LeftAdder += 1;
                    Debug.Log("memeRR" + (HapticLeft + DifferenceLeft) + ":" + DifferenceLeft);
                    HapticImpulseLeft.Adder(HapticLeft);

                }

                Debug.Log("HALLO :D");
                Debug.Log(leftController.position);
                DetectController(leftController, "Left");
            }

            if (innerFeedbackRight)
            {



                if (RightAdder % 10 == 0 && HapticRight < 1.0f && hapticListRight.Count != 0)
                {

                    Debug.Log("MEMEAA" + RightAdder + ":" + hapticListRight.Count);


                    if (hapticListRight.Count == RightAdder / 10)
                    {
                        RightAdder = 0;
                    }


                    float DifferenceRight = hapticListRight[RightAdder / 10] - HapticRight;
                    Debug.Log("memeRR" + (HapticRight + DifferenceRight) + ":" + DifferenceRight);
                    HapticImpulseRight.Adder(DifferenceRight);





                }
                RightAdder += 1;

                Debug.Log("HALLO :D");
                Debug.Log("HEYYY" + rightController.position);
                DetectController(rightController, "Cane");
            }




        }
    }

    private IEnumerator VibrationVariation(Collision collision)
    {
        GameObject collidedObject = LocateCollidedObjectRoot(collision.gameObject);


        string collidedObjectTag = GetObjectName(collidedObject);
        string playerColliderTag = GetObjectName(gameObject);


        if (Collisions.TryGetValue(collidedObjectTag + playerColliderTag, out var itemToUpdate))
        {
            if (collision.gameObject.tag == "floor")
            {

                itemToUpdate.IsColliding = false;

            }
        }

        while (itemToUpdate.IsColliding)
        {
            yield return new WaitForSeconds(0.15f);

            if (innerFeedbackLeft)
            {

                if (LeftAdder % 10 == 0 && HapticLeft < 1.0f - ForceCutLeft && hapticListLeft.Count != 0)
                {

                    if (hapticListLeft.Count == LeftAdder / 10)
                    {
                        LeftAdder = 0;
                    }
                    float DifferenceLeft = hapticListLeft[LeftAdder / 10] - HapticLeft;

                    Debug.Log("memebughaptic" + HapticLeft);
                    LeftAdder += 1;
                    Debug.Log("memeRR" + (HapticLeft + DifferenceLeft) + ":" + DifferenceLeft);
                    HapticImpulseLeft.Adder(HapticLeft);

                }
            }

            if (innerFeedbackRight)
            {



                if (RightAdder % 10 == 0 && HapticRight < 1.0f && hapticListRight.Count != 0)
                {


                    if (hapticListRight.Count == RightAdder / 10)
                    {
                        RightAdder = 0;
                    }


                    float DifferenceRight = hapticListRight[RightAdder / 10] - HapticRight;
                    Debug.Log("memeRR" + (HapticRight + DifferenceRight) + ":" + DifferenceRight);
                    HapticImpulseRight.Adder(DifferenceRight);





                }
                RightAdder += 1;
            }

        }

        

    }


    #endregion
}
