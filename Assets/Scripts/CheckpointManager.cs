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
            playerController.enabled = false;
        }

        if (playerCamera != null)
        {
            playerCamera.enabled = false;
        }

        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            player.transform.position = playerCheckpointPosition;
            rb.isKinematic = false;
        }
        else
        {
            player.transform.position = playerCheckpointPosition;
        }

        foreach (EnemyStateMachine enemy in allEnemies)
        {
            if (enemy != null)
            {
                enemy.ResetToInitialState();
            }
        }

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
        });
    }
}
