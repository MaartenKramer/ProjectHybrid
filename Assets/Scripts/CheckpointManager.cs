using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;

    private Vector3 playerCheckpointPosition;
    private EnemyStateMachine[] allEnemies;

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

        // Get all enemies with EnemyStateMachine in the scene
        allEnemies = UnityEngine.Object.FindObjectsByType<EnemyStateMachine>(FindObjectsSortMode.None);
    }

    public void SetCheckpoint(Vector3 position)
    {
        playerCheckpointPosition = position;
        Debug.Log($"Checkpoint set at position: {position}");
    }

    public void ResetToCheckpoint(GameObject player)
    {
        // Disable player controls
        PlayerController playerController = player.GetComponent<PlayerController>();
        PlayerCamera playerCamera = player.GetComponentInChildren<PlayerCamera>();

        if (playerController != null)
        {
            playerController.enabled = false;
        }

        if (playerCamera != null)
        {
            playerCamera.enabled = false;
        }

        // Reset player position and physics
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true; // Temporarily disable physics for reset
        }

        player.transform.position = playerCheckpointPosition;

        if (rb != null)
        {
            rb.isKinematic = false; // Re-enable physics
        }

        // Reset all enemies to their initial state
        foreach (EnemyStateMachine enemy in allEnemies)
        {
            if (enemy != null)
            {
                enemy.ResetToInitialState();
            }
            else
            {
                Debug.LogWarning("An enemy in the allEnemies array is null. It might have been destroyed.");
            }
        }

        // Fade back in and re-enable player controls
        FadeManager.Instance.FadeIn(() =>
        {
            Debug.Log("Fade completed. Re-enabling player components.");
            if (playerController != null)
            {
                playerController.enabled = true;
            }

            if (playerCamera != null)
            {
                playerCamera.enabled = true;
            }
        });
    }
}
