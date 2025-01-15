using UnityEngine;
using UnityEngine.Playables;

public class TriggerCutSceneScript : MonoBehaviour
{
    public PlayableDirector timeline; // Reference to the PlayableDirector
    public bool oneTimeTrigger = true; // Ensures the cutscene only plays once

    private bool hasTriggered = false; // Tracks if the cutscene has already played

    void Start()
    {
        // Stop the timeline at the start of the scene
        if (timeline != null)
        {
            timeline.Stop();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is tagged as "Player"
        if (other.CompareTag("Player") && !hasTriggered)
        {
            TriggerCutscene();
        }
    }

    void TriggerCutscene()
    {
        if (timeline != null)
        {
            timeline.Play(); // Play the Timeline
            Debug.Log("Cutscene started!");

            if (oneTimeTrigger)
            {
                hasTriggered = true;
            }
        }
        else
        {
            Debug.LogWarning("PlayableDirector (Timeline) is not assigned.");
        }
    }
}
