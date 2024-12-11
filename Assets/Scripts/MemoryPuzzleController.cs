using System.Collections.Generic;
using UnityEngine;

public class MemoryPuzzleController : MonoBehaviour
{
    public static MemoryPuzzleController Instance;

    [Header("Tile Sequence")]
    public List<int> correctOrder; // Define the sequence of tile IDs in the Inspector

    [Header("Audio Clips")]
    public AudioClip correctSound;
    public AudioClip incorrectSound;
    public AudioClip solvedSound;

    private int currentStep = 0;
    private Tile lastFailedTile = null;
    private bool puzzleCompleted = false;
    private Tile[] allTiles;
    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        allTiles = UnityEngine.Object.FindObjectsByType<Tile>(FindObjectsSortMode.None);
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void CheckTile(int tileID, Tile tile)
    {
        if (puzzleCompleted) return;
        if (lastFailedTile == tile) return;

        if (currentStep < correctOrder.Count && tileID == correctOrder[currentStep])
        {
            tile.ActivateTile();
            PlaySound(correctSound);
            currentStep++;

            if (currentStep >= correctOrder.Count)
            {
                CompletePuzzle();
            }
        }
        else
        {
            FailPuzzle(tile);
        }
    }

    private void CompletePuzzle()
    {
        puzzleCompleted = true;
        PlaySound(solvedSound);
        UnityEngine.Debug.Log("Puzzle solved!");
    }

    private void FailPuzzle(Tile tile)
    {
        lastFailedTile = tile;
        PlaySound(incorrectSound);
        ResetPuzzle();
    }

    private void ResetPuzzle()
    {
        currentStep = 0;
        lastFailedTile = null;

        foreach (Tile tile in allTiles)
        {
            tile.ResetTile();
        }

        UnityEngine.Debug.Log("Puzzle reset!");
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
