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

        allEnemies = UnityEngine.Object.FindObjectsByType<EnemyStateMachine>(FindObjectsSortMode.None);
    }

    public void SetCheckpoint(Vector3 position)
    {
        playerCheckpointPosition = position;
        UnityEngine.Debug.Log($"Checkpoint set at position: {position}");
    }

    public void ResetToCheckpoint(GameObject player)
    {
        PlayerController playerController = player.GetComponent<PlayerController>();
        PlayerCamera playerCamera = player.GetComponentInChildren<PlayerCamera>();

        if (playerController != null)
        {
            playerController.SetControlsEnabled(false);
        }

        if (playerCamera != null)
        {
            playerCamera.SetCameraEnabled(false);
        }

        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            player.transform.position = playerCheckpointPosition;
            rb.isKinematic = false;
        }
        else
        {
            player.transform.position = playerCheckpointPosition;
        }

        if (playerController != null)
        {
            playerController.SetControlsEnabled(true);
        }

        if (playerCamera != null)
        {
            playerCamera.SetCameraEnabled(true);
        }
    }
}
