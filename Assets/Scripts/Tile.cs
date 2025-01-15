using UnityEngine;

public class Tile : MonoBehaviour
{
    public int tileID;

    private Light tileLight;

    private Color greenColor = new Color(0.52f, 1f, 0.38f); // Converted from #84FF62
    private Color redColor = Color.red;

    private void Start()
    {
        tileLight = GetComponentInChildren<Light>();
        if (tileLight != null)
        {
            tileLight.enabled = false; // Ensure the light is off initially
            tileLight.color = greenColor; // Set initial color to green
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
        if (tileLight != null)
        {
            tileLight.enabled = true; // Enable the light when activated
        }
    }

    public void ResetTile()
    {
        if (tileLight != null)
        {
            tileLight.enabled = false; // Turn off the light
            tileLight.color = greenColor; // Reset to green
        }
    }

    public void SetLightColor(Color color, bool keepOn)
    {
        if (tileLight != null)
        {
            tileLight.color = color;
            if (keepOn)
            {
                tileLight.enabled = true;
            }
        }
    }
}
