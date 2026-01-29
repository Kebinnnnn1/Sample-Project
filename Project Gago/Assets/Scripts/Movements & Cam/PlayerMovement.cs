using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 4f;
    public float sprintSpeed = 7f;
    public float crouchSpeed = 2f;
    public float acceleration = 10f;

    [Header("Jumping & Gravity")]
    public float jumpHeight = 1.5f;
    public float gravity = -20f;
    public float autoJumpCheckDistance = 0.6f;
    public LayerMask obstacleLayer;

    [Header("Crouch")]
    public float crouchHeight = 1f;
    public float standingHeight = 2f;
    public float crouchSmooth = 8f;

    [Header("Physics Interaction")]
    public float pushForce = 5f;

    CharacterController controller;
    Vector3 velocity;
    Vector3 moveVelocity;

    bool isCrouching;

    // 🔑 jump lock
    bool canJump = true;

    MovingPlatform currentPlatform;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        Crouch();
        Move();
        GravityAndJump();
        AutoJump();
        ApplyPlatformMovement();
    }

    // =============================
    // MOVEMENT
    // =============================
    void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 input = (transform.right * x + transform.forward * z).normalized;

        bool sprinting =
            Input.GetKey(KeyCode.LeftShift) &&
            !isCrouching &&
            input.magnitude > 0.1f;

        float speed = walkSpeed;
        if (isCrouching) speed = crouchSpeed;
        else if (sprinting) speed = sprintSpeed;

        Vector3 targetVelocity = input * speed;
        moveVelocity = Vector3.Lerp(
            moveVelocity,
            targetVelocity,
            acceleration * Time.deltaTime
        );

        controller.Move(moveVelocity * Time.deltaTime);
    }

    // =============================
    // GRAVITY & JUMP (CORRECT)
    // =============================
    void GravityAndJump()
    {
        // jump (single only)
        if (Input.GetKeyDown(KeyCode.Space) && canJump && !isCrouching)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            canJump = false;
        }

        velocity.y += gravity * Time.deltaTime;

        // move + get collision result
        CollisionFlags flags = controller.Move(velocity * Time.deltaTime);

        // landed ONLY if we hit something below
        if ((flags & CollisionFlags.Below) != 0)
        {
            velocity.y = -2f;
            canJump = true;
        }
    }

    // =============================
    // CROUCH
    // =============================
    void Crouch()
    {
        isCrouching = Input.GetKey(KeyCode.LeftControl);

        float targetHeight = isCrouching ? crouchHeight : standingHeight;
        controller.height = Mathf.Lerp(
            controller.height,
            targetHeight,
            crouchSmooth * Time.deltaTime
        );
    }

    // =============================
    // AUTO JUMP (SAFE)
    // =============================
    void AutoJump()
    {
        if (!canJump || isCrouching || moveVelocity.magnitude < 0.1f)
            return;

        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up * 0.3f;

        if (Physics.Raycast(origin, transform.forward, out hit, autoJumpCheckDistance, obstacleLayer))
        {
            float obstacleHeight = hit.collider.bounds.max.y - transform.position.y;
            if (obstacleHeight > 0.3f && obstacleHeight < 1.2f)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                canJump = false;
            }
        }
    }

    // =============================
    // PUSH RIGIDBODIES
    // =============================
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody rb = hit.collider.attachedRigidbody;
        if (rb == null || rb.isKinematic) return;
        if (hit.moveDirection.y < -0.3f) return;
        if (moveVelocity.magnitude < 0.1f) return;

        Vector3 pushDir =
            new Vector3(hit.moveDirection.x, 0f, hit.moveDirection.z).normalized;

        rb.AddForce(pushDir * pushForce, ForceMode.Force);
    }

    void ApplyPlatformMovement()
    {
        if (currentPlatform == null) return;
        controller.Move(currentPlatform.DeltaMovement);
    }

    // =============================
    // PUBLIC STATES
    // =============================
    public bool IsGrounded => canJump; // logical grounded
    public bool IsMoving => moveVelocity.magnitude > 0.1f;
    public bool IsCrouching => isCrouching;
    public bool IsSprinting =>
        Input.GetKey(KeyCode.LeftShift) &&
        !isCrouching &&
        IsMoving;
}
