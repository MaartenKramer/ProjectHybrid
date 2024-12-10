using UnityEngine;

public class Tile : MonoBehaviour
{
    public int tileID;
    private Renderer tileRenderer;
    private Color originalColor;

    private void Start()
    {
        tileRenderer = GetComponent<Renderer>();
        if (tileRenderer != null)
        {
            originalColor = tileRenderer.material.color;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MemoryPuzzleController.Instance.CheckTile(tileID, this);
        }
    }

    public void ActivateTile()
    {
        if (tileRenderer != null)
        {
            tileRenderer.material.color = originalColor * 1.5f;
        }
    }

    public void ResetTile()
    {
        if (tileRenderer != null)
        {
            tileRenderer.material.color = originalColor;
        }
    }
}
