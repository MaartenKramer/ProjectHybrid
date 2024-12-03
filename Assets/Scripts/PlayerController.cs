using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerCharacter : MonoBehaviour
{
    public float walkSpeed = 5f;       // Normal walking speed
    public float sprintSpeed = 15f;   // Sprinting speed
    public float rotationSpeed = 720f; // Rotation speed (degrees per second)
    public float acceleration = 8f;   // Speed up factor
    public float deceleration = 10f;  // Slow down factor
    public float jumpForce = 8f;      // Jump height
    public float gravity = -20f;      // Custom gravity force
    public Transform cameraTransform; // Assign the camera in the Inspector

    private Rigidbody rb;
    private Vector3 currentVelocity;  // Tracks the player's movement velocity
    private Vector3 verticalVelocity; // Tracks vertical movement (jumping/falling)
    private bool isSprinting;         // Tracks whether the player is sprinting

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Prevent physics-based rotation
    }

    private void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        // Input for movement
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Check if the player is sprinting
        isSprinting = Input.GetKey(KeyCode.LeftShift);

        // Determine the current speed
        float targetSpeed = isSprinting ? sprintSpeed : walkSpeed;

        // Calculate movement direction
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            // Align movement with the camera's forward direction
            Vector3 moveDirection = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0) * direction;
            moveDirection.y = 0;

            // Smoothly adjust velocity
            currentVelocity = Vector3.Lerp(currentVelocity, moveDirection * targetSpeed, Time.fixedDeltaTime * acceleration);

            // Rotate the player to face the movement direction
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
        else
        {
            // Smooth deceleration when no input
            currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, Time.fixedDeltaTime * deceleration);
        }

        // Gravity and jumping
        if (IsGrounded())
        {
            verticalVelocity.y = 0;

            if (Input.GetButtonDown("Jump"))
            {
                verticalVelocity.y = jumpForce;
            }
        }
        else
        {
            verticalVelocity.y += gravity * Time.fixedDeltaTime;
        }

        // Combine horizontal and vertical movement
        Vector3 finalMovement = currentVelocity + verticalVelocity.y * Vector3.up;

        // Move the player using Rigidbody
        rb.MovePosition(rb.position + finalMovement * Time.fixedDeltaTime);
    }

    bool IsGrounded()
    {
        // Use a raycast to check if the player is grounded
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }
}
