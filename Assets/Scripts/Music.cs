using UnityEngine;

public class Music : MonoBehaviour
{
    public AudioClip track1;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (track1 != null)
        {
            audioSource.clip = track1;
            audioSource.Play();
        }
    }
}
