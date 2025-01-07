using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform target;
    public float distance = 30f;
    public float height = 10f;
    public float rotationSpeed = 5f;
    public float minVerticalAngle = -30f;
    public float maxVerticalAngle = 40f;

    private float yaw;
    private float pitch;

    public void SetCameraEnabled(bool isEnabled)
    {
        enabled = isEnabled;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        yaw += Input.GetAxis("Mouse X") * rotationSpeed;
        pitch = Mathf.Clamp(pitch - Input.GetAxis("Mouse Y") * rotationSpeed, minVerticalAngle, maxVerticalAngle);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 offset = rotation * new Vector3(0, height, -distance);

        transform.position = target.position + offset;
        transform.LookAt(target.position + Vector3.up * height);
    }
}
