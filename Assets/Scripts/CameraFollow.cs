using UnityEngine;


public class CameraFollow : MonoBehaviour
{
    public float smoothSpeed = 0.125f;
    public Vector3 offset;


    private Transform target; // Target transform for the camera to follow


    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;


            transform.LookAt(target);
        }
    }


    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}