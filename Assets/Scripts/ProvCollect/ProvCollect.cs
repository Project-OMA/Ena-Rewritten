using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public class ProvCollect : MonoBehaviour
{
    private float waitTime = 2.0f;

    private float timer = 0.0f;

    private float visualTime = 0.0f;

    private float scrollBar = 1.0f;

    private Transform playerTransform;

    private StreamWriter writer;

    private readonly string fileName = $"{Directory.GetCurrentDirectory()}/PlayerLogs/myfile.txt";

    private readonly string endpointUrl = "";

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = transform;
        writer = new StreamWriter(fileName, true);
    }

    private void SavePositionsToFile(Vector3 pos)
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
            timer -= waitTime;
            Time.timeScale = scrollBar;
            var pos = playerTransform.position;
            Debug.Log(pos);
            SavePositionsToFile(pos);
        }
    }

    private IEnumerator Upload()
    {
        // Create a UnityWebRequest with a POST method
        UnityWebRequest www = UnityWebRequest.Post(endpointUrl, new WWWForm());

        // Create a new MultipartFormDataSection to upload the file
        byte[] fileData = File.ReadAllBytes(fileName);
        www.uploadHandler = new UploadHandlerRaw(fileData)
        {
            contentType = "application/octet-stream"
        };

        // authentication if needed.
        // www.SetRequestHeader("Authorization", "Bearer YourAccessToken");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + www.error);
        }
        else
        {
            Debug.Log("Upload complete! Server response: " + www.downloadHandler.text);
        }
    }

    void OnApplicationQuit()
    {
        writer.Close();
        StartCoroutine(Upload());
    }
}