using System;
using System.Collections.Generic;
using UnityEngine;

public class CollisionEventHandler : MonoBehaviour
{
    public AudioSource alarme;
    public Rigidbody rb;

    public AudioSource Alarme { get => alarme; set => alarme = value; }
    public List<CollisionEvent> History = new List<CollisionEvent>();
    static public List<CollisionEvent> Collisions = new List<CollisionEvent>();
    void Start()
    {
        
    }

    void Update()
    {
        Collisions.RemoveAll(item => {
            try
            {
                ProcessCollisionItem(item);
                return !item.IsActive;
            }
            catch (Exception ex)
            {
                Debug.LogError($"An error occurred: {ex.Message}");
                return false;
            }
        });
    }

    void ProcessCollisionItem(CollisionEvent item)
    {
        if (item.IsActive)
        {
            Debug.Log("Playing");
            Alarme.Play();
            return;
        }
        Debug.Log("Stopping");
        History.Add(item);
        Alarme.Stop();
        return;
    }

    private void StartFeedback(CollisionEvent collision)
    {
        switch (collision.FeedbackType)
        {
            case FeedbackTypeEnum.Alarm:
                AlarmFeedback(collision.IsActive);
                break;
            default:
                break;
        }
    }

    private void AlarmFeedback(bool active)
    {
        if(active)
        {
            Alarme.Play();
            return;
        }
        Alarme.Stop();
    }
}
