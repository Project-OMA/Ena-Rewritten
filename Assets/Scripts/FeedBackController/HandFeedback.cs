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

    private string tagLeft;

    private string tagRight;



    public float ForceCutRight;




    public static readonly Dictionary<string, CollisionEvent> Collisions = new Dictionary<string, CollisionEvent>();


    private readonly string fileName = $"{Directory.GetCurrentDirectory()}/PlayerLogs/feedback.csv";

    #endregion

    #region Properties

    public Dictionary<string, AudioSource> SoundSources { get; private set; }
    public GameObject CaneSource;
    public GameObject WallSource;
    private string map;
    private bool noSoundChild = true;

    public AudioSource audioTrail;

    public static bool innerFeedbackLeft = false;
    public static bool innerFeedbackRight = false;
    private Coroutine feedbackCoroutineLeft = null;
    private Coroutine feedbackCoroutineRight = null;
    public Transform rayPos;

    private float nextUpdate = 0.2f;

    private float differenceLeft;

    private float differenceRight;


    public static bool outLeft = false;

    public static bool outRight = false;





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



    public void HandleCollisionEnter(Collision collision, string controller)
    {

        // Collisions with the Player game object are reported sometimes. This causes problems in the
        // LocateCollidedObjectRoot method, since the Player is located in the scene root (has no parent)
        if (collision.gameObject.tag == "car" || collision.gameObject.tag == "Player") return;

        ContactPoint contact = collision.contacts[0];
        Debug.Log("Pos:" + contact.point);

        // Since objects have their mesh colliders placed in the inner objects in the hierarchy,
        // we have to "move up" the object tree until we find the root object of the prop (which 
        // contains the FeedbackSettings component)
        GameObject collidedObject = LocateCollidedObjectRoot(collision.gameObject);
        string tag = collision.gameObject.tag;

        string collidedObjectTag = GetObjectName(collidedObject);
        string playerColliderTag = GetObjectName(gameObject);

        var feedbackSettings = collidedObject.GetComponent<ObjectFeedbackSettings>()?.settings;

        TutorialCheckpoints.playerHasInteracted = true;




        if (Collisions.TryGetValue(collidedObjectTag + playerColliderTag + controller, out var item))
        {

            playerColliding = true;

            if (HandCheck.LeftHand && !innerFeedbackLeft)
            {

                hapticListLeft = feedbackSettings.hapticValues;
                tagLeft = tag;

                innerFeedbackLeft = true;
            }

            if (HandCheck.RightHand && !innerFeedbackRight)
            {


                hapticListRight = feedbackSettings.hapticValues;
                tagRight = tag;

                innerFeedbackRight = true;
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
                whatcollided: playerColliderTag +"-"+controller,
                feedbackSettings: feedbackSettings,
                gameObject: collidedObject,
                vector3: contact.point,
                totalCollisions: 1,
                currentMap: map);

            Collisions.Add(collidedObjectTag + playerColliderTag + controller, collisionEvent);


            playerColliding = true;


            if (HandCheck.LeftHand && !innerFeedbackLeft)
            {
                Debug.Log("HewoLeft" + collisionEvent.Whatcollided);
                tagLeft = collisionEvent.GameObject.tag;
                innerFeedbackLeft = true;
                hapticListLeft = feedbackSettings.hapticValues;
            }

            if (HandCheck.RightHand && !innerFeedbackRight)
            {
                Debug.Log("HewoRight" + collisionEvent.Whatcollided);
                tagRight = collisionEvent.GameObject.tag;
                innerFeedbackRight = true;
                hapticListRight = feedbackSettings.hapticValues;
            }



            HandleCollisionEnterFeedback(collisionEvent);

        }


    }


    public void HandleCollisionExit(Collision collision, string controller)
    {

        Debug.Log("amberlamps" + collision.gameObject.tag);





        GameObject collidedObject = LocateCollidedObjectRoot(collision.gameObject);


        string collidedObjectTag = GetObjectName(collidedObject);
        string playerColliderTag = GetObjectName(gameObject);

        if (collision.gameObject.tag == "DoorWindow")
        {

            TutorialCheckpoints.playerDoor = true;
        }

        if (Collisions.TryGetValue(collidedObjectTag + playerColliderTag + controller, out var itemToUpdate))
        {
            itemToUpdate.IsColliding = false;
            itemToUpdate.CanPlay = false;

            Debug.Log("BRUHP" + itemToUpdate.IsColliding);
            if (collision.gameObject.tag == "floor")
            {

                switch (controller)
                {

                    case "Right":

                        HapticImpulseRight.Stop();
                        HapticRight = 0.0f;
                        RightAdder = 0;
                        innerFeedbackRight = false;
                        HandCheck.RightHand = false;
                        RightHand.rightInside = false;

                        if (feedbackCoroutineRight != null)
                        {
                            StopCoroutine(feedbackCoroutineRight);
                            feedbackCoroutineRight = null;
                        }

                        break;

                    case "Left":

                        HapticImpulseLeft.Stop();
                        HapticLeft = 0.0f;
                        LeftAdder = 0;
                        innerFeedbackLeft = false;
                        HandCheck.LeftHand = false;
                        LeftHand.leftInside = false;

                        if (feedbackCoroutineLeft != null)
                        {
                            StopCoroutine(feedbackCoroutineLeft);
                            feedbackCoroutineLeft = null;
                        }

                        break;
                }

                playerColliding = false;
            }




        }


    }

    #endregion

    #region Feedback Handling




    private void HandleCollisionEnterFeedback(CollisionEvent item)
    {

        Debug.Log("Hewo" + item.CollidedObject);

        item.IsColliding = true;
        item.CanPlay = true;
        HandleFeedback(item);

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

            if (HandCheck.LeftHand && !HapticImpulseLeft.isHapticFeedbackPlaying)
            {

                HapticLeft = hapticForce;

                HapticImpulseLeft.Play(hapticForce);

            }

            if (HandCheck.RightHand && !HapticImpulseRight.isHapticFeedbackPlaying)
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

    public void DetectControllerLeft()
    {
        RaycastHit hit;
        Vector3 direction = (leftController.position - rayPos.position).normalized;


        Debug.DrawRay(rayPos.position, direction * 5, Color.red, 5, true);

        if (Physics.Raycast(rayPos.position, direction, out hit, 5))
        {




            if (hit.collider.tag == tagLeft && !LeftHand.leftInside)
            {
                Debug.Log("MemeDetect" + hit.collider.gameObject.tag);
                if (feedbackCoroutineLeft == null && tagLeft != "floor")
                {
                    LeftHand.leftInside = true;
                    feedbackCoroutineLeft = StartCoroutine(FeedbackRoutineLeft());
                }

            }

            if (hit.collider.tag == "Left" && (!outLeft && LeftHand.leftInside))
            {
                HapticLeft = 0.25f;

                LeftHand.leftInside = false;

                if (feedbackCoroutineLeft != null)
                {
                    StopCoroutine(feedbackCoroutineLeft);
                    feedbackCoroutineLeft = null;
                }

            }
            

            if (hit.collider.tag == "Left" && outLeft)
            {
                HapticImpulseLeft.Stop();
                outLeft = false;
                HapticLeft = 0.0f;
                LeftAdder = 0;
                innerFeedbackLeft = false;
                HandCheck.LeftHand = false;
                LeftHand.leftInside = false;

                if (feedbackCoroutineLeft != null)
                {
                    StopCoroutine(feedbackCoroutineLeft);
                    feedbackCoroutineLeft = null;
                }

            }
        }
    }

    public void DetectControllerRight()
    {
        RaycastHit hit;
        Vector3 direction = (rightController.position - rayPos.position).normalized;
        Debug.DrawRay(rayPos.position, direction * 5, Color.red, 5, true);


        if (Physics.Raycast(rayPos.position, direction, out hit, 5))
        {

            Debug.Log("TESTRIGHT" + hit.collider.tag);

            if (hit.collider.tag == tagRight && !RightHand.rightInside)
            {
                Debug.Log("TESTRIGHT" + hit.collider.tag + "1");
                if (feedbackCoroutineRight == null && tagRight != "floor")
                {

                    RightHand.rightInside = true;
                    feedbackCoroutineRight = StartCoroutine(FeedbackRoutineRight());
                }

            }

            if (hit.collider.tag == "Right" && (!outRight && RightHand.rightInside))
            {
                HapticRight = 0.25f;

                RightHand.rightInside = false;
                
                if (feedbackCoroutineRight != null)
                {
                    StopCoroutine(feedbackCoroutineRight);
                    feedbackCoroutineRight = null;
                }

            }

            if (hit.collider.tag == "Cane" && outRight)
            {
                HapticImpulseRight.Stop();
                outRight = false;
                HapticRight = 0.0f;
                RightAdder = 0;
                innerFeedbackRight = false;
                HandCheck.RightHand = false;
                RightHand.rightInside = false;

                if (feedbackCoroutineRight != null)
                {
                    StopCoroutine(feedbackCoroutineRight);
                    feedbackCoroutineRight = null;
                }
            }
        }

    }

    private IEnumerator FeedbackRoutineLeft()
    {
        float incrementStep = 0.05f;
        HapticLeft = HapticLeft + differenceLeft;


        while (HandCheck.LeftHand)
        {
            yield return new WaitForSeconds(0.05f);

            if (HapticLeft < 1.0f)
            {
                HapticLeft += incrementStep;
                HapticLeft = Mathf.Min(HapticLeft, 1.0f);

                float differenceLeft = 1.0f - HapticLeft;
                Debug.Log("Updated HapticLeft: " + HapticLeft);


                HapticImpulseLeft.Adder(differenceLeft);


            }

            DetectControllerLeft();
        }



        feedbackCoroutineLeft = null;
    }

    private IEnumerator FeedbackRoutineRight()
    {
        float incrementStep = 0.05f;
        HapticRight = HapticRight + differenceRight;

        while (HandCheck.RightHand)
        {
            yield return new WaitForSeconds(0.05f);

            if (HapticRight < 1.0f)
            {
                HapticRight += incrementStep;
                HapticRight = Mathf.Min(HapticRight, 1.0f);

                float differenceRight = 1.0f - HapticRight;
                Debug.Log("Updated HapticRight: " + HapticRight);


                HapticImpulseRight.Adder(differenceRight);


            }

            DetectControllerRight();
        }

        feedbackCoroutineRight = null;
    }



    public void VibrationVariationLeft()
    {
        if (ControllerDetector.canAlternateLeft)
        {
            if (HapticLeft < 1.0f && hapticListLeft.Count > 0)
            {

                if (LeftAdder >= hapticListLeft.Count)
                {
                    LeftAdder = 0;

                }

                differenceLeft = Mathf.Abs(hapticListLeft[LeftAdder]) - HapticLeft;
                Debug.Log("Left Δ: " + differenceLeft + "-" + HapticLeft + "-" + hapticListLeft[LeftAdder]);
                HapticImpulseLeft.Adder(differenceLeft);
            }
            LeftAdder++;
        }

        
    }
    
    public void VibrationVariationRight()
    {

        if (ControllerDetector.canAlternateRight)
        {
            if (HapticRight < 1.0f && hapticListRight.Count > 0)
            {

                if (RightAdder >= hapticListRight.Count)
                {
                    RightAdder = 0;

                }

                differenceRight = Mathf.Abs(hapticListRight[RightAdder]) - HapticRight;
                Debug.Log("Right Δ: " + differenceRight + "-" + HapticRight + "-" + hapticListRight[RightAdder]);
                HapticImpulseRight.Adder(differenceRight);
            }
            RightAdder++;
        }
    }



    


    #endregion
}
