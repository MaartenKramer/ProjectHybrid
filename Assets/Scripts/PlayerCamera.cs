using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform target;
    public float distance = 30f;
    public float height = 10f;
    public float rotationSpeed = 5f;
    public float minVerticalAngle = -30f;
    public float maxVerticalAngle = 40f;

    private float currentYaw = 0f;
    private float currentPitch = 0f;

    private void LateUpdate()
    {
        if (!target) return;

        float horizontalInput = Input.GetAxis("Mouse X") * rotationSpeed;
        float verticalInput = -Input.GetAxis("Mouse Y") * rotationSpeed;

        currentYaw += horizontalInput;
        currentPitch += verticalInput;

        currentPitch = Mathf.Clamp(currentPitch, minVerticalAngle, maxVerticalAngle);

        Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0);
        Vector3 offset = rotation * new Vector3(0, height, -distance);

        transform.position = target.position + offset;
        transform.LookAt(target.position + Vector3.up * height);
    }
}
