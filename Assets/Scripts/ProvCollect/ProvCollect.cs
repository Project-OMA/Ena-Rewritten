using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;
using System.IO;

public class ProvCollect : MonoBehaviour
{
    private List<Vector3> playerPositions = new List<Vector3>();
    private float waitTime = 2.0f;

    private float timer = 0.0f;

    private float visualTime = 0.0f;

    private float scrollBar = 1.0f;

    private Transform playerTransform;    

    private StreamWriter writer;

    
    
    private string fileName = $"{Directory.GetCurrentDirectory()}/PlayerLogs/myfile.txt";

    private string endpointUrl = "";
    // Start is called before the first frame update
    void Start()
    {
        
        playerTransform = transform;
        writer = new StreamWriter(fileName, true);
    }

    private void SavePositionsToFile(Vector3 pos, string fileName)
    {
        
            
            writer.WriteLine(pos.ToString());
            
        
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
            SavePositionsToFile(pos, fileName);

        }
        
    }

    private IEnumerator Upload()
    {

        // Create a UnityWebRequest with a POST method
        UnityWebRequest www = UnityWebRequest.Post(endpointUrl, "POST");

        // Create a new MultipartFormDataSection to upload the file
        byte[] fileData = File.ReadAllBytes(fileName);
        www.uploadHandler = new UploadHandlerRaw(fileData);
        www.uploadHandler.contentType = "application/octet-stream";

        // Set headers (if needed)
        www.SetRequestHeader("Authorization", "Bearer YourAccessToken");

        // Send the request
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogError("Error: " + www.error);
        }
        else
        {
            Debug.Log("Upload complete! Server response: " + www.downloadHandler.text);
        }
    }

    void OnApplicationQuit(){
        writer.Close();
        //Upload();
    }
}
