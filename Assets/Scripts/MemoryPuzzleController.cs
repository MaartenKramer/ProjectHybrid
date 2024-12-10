using UnityEngine;

public class MemoryPuzzleController : MonoBehaviour
{
    public static MemoryPuzzleController Instance;
    private readonly int[] correctOrder = { 2, 1, 3, 4 };
    private int currentStep = 0;
    private Tile lastFailedTile = null;
    private bool puzzleCompleted = false;
    private Tile[] allTiles;

    [Header("Audio Clips")]
    public AudioClip correctSound;
    public AudioClip incorrectSound;
    public AudioClip solvedSound;

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

        if (tileID == correctOrder[currentStep])
        {
            tile.ActivateTile();
            PlaySound(correctSound);
            currentStep++;

            if (currentStep >= correctOrder.Length)
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
