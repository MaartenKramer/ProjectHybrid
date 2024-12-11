using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float sprintSpeed = 15f;
    public float rotationSpeed = 720f;
    public float acceleration = 8f;
    public float deceleration = 10f;
    public float jumpForce = 8f;
    public float gravity = -20f;
    public Transform cameraTransform;
    public float groundCheckDistance = 1.1f;

    private Rigidbody rb;
    private Vector3 currentVelocity;
    private float verticalVelocity;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        float targetSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;

        Vector3 inputDirection = new Vector3(horizontal, 0, vertical).normalized;

        if (inputDirection.magnitude >= 0.1f)
        {
            Vector3 moveDirection = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0) * inputDirection;

            currentVelocity = moveDirection * targetSpeed;

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
        else
        {
            currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, Time.fixedDeltaTime * deceleration);
        }

        if (IsGrounded())
        {
            verticalVelocity = 0;

            if (Input.GetButtonDown("Jump"))
            {
                verticalVelocity = jumpForce;
            }
        }
        else
        {
            verticalVelocity += gravity * Time.fixedDeltaTime;
        }

        Vector3 finalMovement = currentVelocity + Vector3.up * verticalVelocity;
        rb.MovePosition(rb.position + finalMovement * Time.fixedDeltaTime);
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);
    }
}
