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
    PlayerController playerController = player.GetComponent<PlayerController>();
    PlayerCamera playerCamera = player.GetComponentInChildren<PlayerCamera>();
    Rigidbody rb = player.GetComponent<Rigidbody>();

    if (playerController != null)
    {
        playerController.enabled = false;
    }

    if (playerCamera != null)
    {
        playerCamera.enabled = false;
    }

    if (rb != null)
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true; // Ensure physics is locked
    }

    player.transform.position = playerCheckpointPosition;

    Debug.Log("Player position and controls have been reset.");

    FadeManager.Instance.FadeIn(() =>
    {
        if (playerController != null)
        {
            playerController.enabled = true;
        }

        if (playerCamera != null)
        {
            playerCamera.enabled = true;
        }

        if (rb != null)
        {
            rb.isKinematic = false; // Re-enable physics
        }

        Debug.Log("Player controls re-enabled after fade.");
    });
}
}
