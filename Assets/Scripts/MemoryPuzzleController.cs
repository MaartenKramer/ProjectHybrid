using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryPuzzleController : MonoBehaviour
{
    public static MemoryPuzzleController Instance;

    [Header("Tile Sequence")]
    public List<int> correctOrder;

    [Header("Audio Clips")]
    public AudioClip correctSound;
    public AudioClip incorrectSound;
    public AudioClip solvedSound;

    [Header("Objects to Fade Out")]
    public List<GameObject> stones;
    public List<GameObject> invisibleWalls;
    public List<GameObject> reaction;
    public List<GameObject> endingRemark;
    public float fadeDuration = 2f;

    private int currentStep = 0;
    private Tile lastFailedTile = null;
    private bool puzzleCompleted = false;
    private bool puzzleLocked = false;
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
        if (puzzleCompleted || puzzleLocked) return;
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
        Debug.Log("Puzzle solved!");

        StartCoroutine(FadeOutObjects());
    }

    private void FailPuzzle(Tile tile)
    {
        lastFailedTile = tile;
        PlaySound(incorrectSound);
        StartCoroutine(ShowFailureSequence());
    }

    private IEnumerator FadeOutObjects()
    {
        float elapsedTime = 0f;

        // Get the initial materials and colors
        Dictionary<Renderer, Color> initialColors = new Dictionary<Renderer, Color>();

        foreach (GameObject obj in stones)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                initialColors[renderer] = renderer.material.color;
            }
        }

        foreach (GameObject obj in invisibleWalls)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                initialColors[renderer] = renderer.material.color;
            }
        }

        // Fade out effect
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);

            foreach (var pair in initialColors)
            {
                Renderer renderer = pair.Key;
                Color color = pair.Value;
                color.a = alpha;
                renderer.material.color = color;
            }

            yield return null;
        }

        // Disable objects
        foreach (GameObject obj in stones)
        {
            obj.SetActive(false);
        }

        foreach (GameObject obj in endingRemark)
        {
            obj.SetActive(true);
        }

        foreach (GameObject obj in invisibleWalls)
        {
            obj.SetActive(false);
        }

        Debug.Log("Objects faded out and disabled.");
    }

    private IEnumerator ShowFailureSequence()
    {
        puzzleLocked = true;

        // Set all lights to red and turn them on
        foreach (Tile tile in allTiles)
        {
            tile.SetLightColor(Color.red, true);
        }

        foreach (GameObject obj in reaction)
        {
            obj.SetActive(true);
        }

        yield return new WaitForSeconds(4f);

        foreach (GameObject obj in reaction)
        {
            obj.SetActive(false);
        }

        // Reset puzzle and turn off all lights
        ResetPuzzle();

        puzzleLocked = false;
    }

    private void ResetPuzzle()
    {
        currentStep = 0;
        lastFailedTile = null;

        foreach (Tile tile in allTiles)
        {
            tile.ResetTile();
        }

        Debug.Log("Puzzle reset!");
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
