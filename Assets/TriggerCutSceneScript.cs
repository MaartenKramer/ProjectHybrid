using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using System.Collections;

public class TriggerCutSceneScript : MonoBehaviour
{
    public PlayableDirector timeline; // Reference to the PlayableDirector
    public bool oneTimeTrigger = true; // Ensures the cutscene only plays once
    public string targetSceneName = "ENDscene"; // Name of the scene to load after the cutscene
    public string requiredUID = "A0B7C70E"; // The UID that is required to trigger the cutscene

    private bool hasTriggered = false; // Tracks if the cutscene has already played
    public SerialController serialController; // Reference to the SerialController

    void Start()
    {
        // Stop the timeline at the start of the scene
        if (timeline != null)
        {
            timeline.Stop();
        }

        // Find the SerialController in the scene
        serialController = GameObject.Find("SerialController").GetComponent<SerialController>();
    }

    void Update()
    {
        // Continuously check for NFC data
        if (hasTriggered)
            return;

        string message = serialController.ReadSerialMessage();

        if (!string.IsNullOrEmpty(message))
        {
            Debug.Log("Raw message received: " + message);
            ProcessData(message);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Only allow triggering the cutscene if the player enters the box collider
        if (other.CompareTag("Player") && !hasTriggered)
        {
            Debug.Log("Player entered trigger area.");
        }
    }

    void ProcessData(string data)
    {
        string trimmedData = ExtractUID(data);
        Debug.Log("Extracted UID: " + trimmedData);

        if (trimmedData == requiredUID && !hasTriggered)
        {
            StartCoroutine(TriggerCutscene());
        }
        else
        {
            Debug.LogWarning("Unexpected UID or non-UID data: " + trimmedData);
        }
    }

    IEnumerator TriggerCutscene()
    {
        if (timeline != null)
        {
            timeline.Play(); // Play the Timeline
            Debug.Log("Cutscene started!");

            if (oneTimeTrigger)
            {
                hasTriggered = true;
            }

            // Wait for the timeline to finish
            yield return new WaitForSeconds(6f);

            // Transition to the ENDscene
            Debug.Log("Transitioning to ENDscene...");
            SceneManager.LoadScene(targetSceneName);
        }
        else
        {
            Debug.LogWarning("PlayableDirector (Timeline) is not assigned.");
        }
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
