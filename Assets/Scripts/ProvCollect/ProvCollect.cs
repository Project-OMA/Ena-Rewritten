using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProvCollect : MonoBehaviour
{
    private List<Vector3> playerPositions = new List<Vector3>();
    private float waitTime = 2.0f;

    private float timer = 0.0f;

    private float visualTime = 0.0f;

    private float scrollBar = 1.0f;

    private Transform playerTransform;    
    // Start is called before the first frame update
    void Start()
    {
        playerTransform = transform;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer > waitTime)
        {
            visualTime = timer;

            // Remove the recorded 2 seconds.
            timer = timer - waitTime;
            Time.timeScale = scrollBar;
            var pos = playerTransform.position;
            Debug.Log(pos);
            playerPositions.Add(pos);

        }
        
    }
}
