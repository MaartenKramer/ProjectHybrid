using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class NFCReader : MonoBehaviour
{
    public SerialController serialController;
    private bool hasTransitioned;

    void Start()
    {
        serialController = GameObject.Find("SerialController").GetComponent<SerialController>();
    }

    void Update()
    {
        string message = serialController.ReadSerialMessage();

        if (string.IsNullOrEmpty(message) || hasTransitioned)
            return;

        Debug.Log("Raw message received: " + message);
        ProcessData(message);
    }

    void ProcessData(string data)
    {
        string trimmedData = ExtractUID(data);
        Debug.Log("Extracted UID: " + trimmedData);

        if (trimmedData == "434BF213")
        {
            LoadScene("EnemyTest");
        }
        else
        {
            Debug.LogWarning("Unexpected UID or non-UID data: " + trimmedData);
        }
    }

    void LoadScene(string sceneName)
    {
        Debug.Log($"Loading scene: {sceneName}");
        hasTransitioned = true;
        SceneManager.LoadScene(sceneName);
    }

    string ExtractUID(string data)
    {
        int colonIndex = data.IndexOf(':');
        if (colonIndex >= 0 && colonIndex + 1 < data.Length)
        {
            string uidPart = data.Substring(colonIndex + 1).Replace(" ", "").Trim();
            return uidPart;
        }
        return data.Replace(" ", "").Trim();
    }
}
