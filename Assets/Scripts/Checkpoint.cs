using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Vector3 checkpointPosition = this.transform.position;
            Debug.Log($"Player reached checkpoint at position: {checkpointPosition}");
            CheckpointManager.Instance.SetCheckpoint(checkpointPosition);
        }
    }
}
