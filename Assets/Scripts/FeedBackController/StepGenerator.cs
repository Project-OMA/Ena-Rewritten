using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepGenerator : MonoBehaviour
{
    public Transform cam;
    public FeedbackController feedbackController;

    private Vector3 previousPosition;
    private int frameCounter;

    
    public float stepThreshold = 0.05f;


    public int frameCheckRate = 10;

    void Start()
    {
        feedbackController = GetComponent<FeedbackController>();
        previousPosition = cam.position;
    }

    void Update()
    {
        frameCounter++;

        if (frameCounter % frameCheckRate != 0) return;

        Vector3 flatPrevious = new Vector3(previousPosition.x, 0f, previousPosition.z);
        Vector3 flatCurrent = new Vector3(cam.position.x, 0f, cam.position.z);
        float distance = Vector3.Distance(flatPrevious, flatCurrent);

        Debug.Log($"Step Movement: {distance}");

        if (distance > stepThreshold)
        {
            feedbackController.handleStep();
        }

        previousPosition = cam.position;
    }
}
