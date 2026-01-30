using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerMovement2 : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 4f;
    public float sprintSpeed = 7f;
    public float crouchSpeed = 2f;

    [Tooltip("How fast we accelerate toward target speed")]
    public float groundAcceleration = 25f;
    public float airAcceleration = 8f;

    [Header("Jumping & Gravity")]
    public float jumpHeight = 1.5f;
    public float gravity = -25f;

    [Header("Double Jump")]
    public int maxJumps = 2;

    [Header("Crouch")]
    public float crouchHeight = 1f;
    public float standingHeight = 2f;
    public float crouchSmooth = 8f;

    [Header("Ground Check")]
    public float groundCheckDistance = 0.25f;
    public LayerMask groundLayer;

    [Header("Jump Reliability")]
    public float coyoteTime = 0.15f;

    Rigidbody rb;
    CapsuleCollider col;

    Vector3 inputDir;
    bool isGrounded;
    bool isCrouching;

    float coyoteTimer;
    int jumpsRemaining;

    MovingPlatform currentPlatform;
    Vector3 platformLastPos;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();

        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        col.height = standingHeight;
        col.center = Vector3.up * standingHeight * 0.5f;

        jumpsRemaining = maxJumps;
    }

    void Update()
    {
        ReadInput();
        GroundCheck();
        Crouch();
        HandleJump();
    }

    void FixedUpdate()
    {
        MoveSmooth();
        ApplyExtraGravity();
        ApplyPlatformMotion();
    }

    // =============================
    // INPUT
    // =============================
    void ReadInput()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        inputDir = (transform.right * x + transform.forward * z).normalized;
    }

    // =============================
    // SMOOTH MOVEMENT
    // =============================
    void MoveSmooth()
    {
        float speed = walkSpeed;

        if (isCrouching) speed = crouchSpeed;
        else if (Input.GetKey(KeyCode.LeftShift) && inputDir.magnitude > 0.1f)
            speed = sprintSpeed;

        Vector3 targetVelocity = inputDir * speed;
        Vector3 currentVelocity = rb.velocity;

        Vector3 horizontalVel = new Vector3(currentVelocity.x, 0f, currentVelocity.z);
        Vector3 velocityDiff = targetVelocity - horizontalVel;

        float accel = isGrounded ? groundAcceleration : airAcceleration;

        rb.AddForce(velocityDiff * accel, ForceMode.Acceleration);

        // Clamp horizontal speed (prevents sprint jitter)
        Vector3 clamped = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if (clamped.magnitude > speed)
        {
            clamped = clamped.normalized * speed;
            rb.velocity = new Vector3(clamped.x, rb.velocity.y, clamped.z);
        }
    }

    // =============================
    // JUMP + DOUBLE JUMP
    // =============================
    void HandleJump()
    {
        if (isGrounded)
        {
            coyoteTimer = coyoteTime;
            jumpsRemaining = maxJumps;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space) &&
            jumpsRemaining > 0 &&
            !isCrouching &&
            jumpHeight > 0f)
        {
            if (!isGrounded && jumpsRemaining == maxJumps && coyoteTimer <= 0f)
                return;

            float jumpVel = Mathf.Sqrt(jumpHeight * -2f * gravity);
            rb.velocity = new Vector3(rb.velocity.x, jumpVel, rb.velocity.z);

            jumpsRemaining--;
            coyoteTimer = 0f;
        }
    }

    // =============================
    // GRAVITY (BETTER FALL)
    // =============================
    void ApplyExtraGravity()
    {
        if (!isGrounded)
            rb.AddForce(Vector3.up * gravity, ForceMode.Acceleration);
    }

    // =============================
    // GROUND CHECK
    // =============================
    void GroundCheck()
    {
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        float radius = col.radius * 0.95f;

        isGrounded = Physics.SphereCast(
            origin,
            radius,
            Vector3.down,
            out RaycastHit hit,
            col.bounds.extents.y + groundCheckDistance,
            groundLayer
        );

        if (isGrounded)
        {
            currentPlatform = hit.collider.GetComponentInParent<MovingPlatform>();
            platformLastPos = currentPlatform
                ? currentPlatform.transform.position
                : Vector3.zero;
        }
        else
        {
            currentPlatform = null;
        }
    }

    // =============================
    // CROUCH
    // =============================
    void Crouch()
    {
        isCrouching = Input.GetKey(KeyCode.LeftControl);

        float targetHeight = isCrouching ? crouchHeight : standingHeight;
        col.height = Mathf.Lerp(col.height, targetHeight, crouchSmooth * Time.deltaTime);
        col.center = Vector3.up * col.height * 0.5f;
    }

    // =============================
    // MOVING PLATFORM
    // =============================
    void ApplyPlatformMotion()
    {
        if (currentPlatform == null) return;

        Vector3 delta = currentPlatform.transform.position - platformLastPos;
        rb.position += delta;
        platformLastPos = currentPlatform.transform.position;
    }

    // =============================
    // WALL STICK FIX
    // =============================
    void OnCollisionStay(Collision collision)
    {
        if (isGrounded) return;

        foreach (var contact in collision.contacts)
        {
            Vector3 normal = contact.normal;

            // Wall (mostly horizontal normal)
            if (Mathf.Abs(normal.y) < 0.2f)
            {
                Vector3 vel = rb.velocity;
                Vector3 intoWall = Vector3.Project(vel, -normal);
                rb.velocity = vel - intoWall;
            }
        }
    }

    // =============================
    // PUBLIC STATES
    // =============================
    public bool IsGrounded => isGrounded;
    public bool IsMoving => inputDir.magnitude > 0.1f;
    public bool IsCrouching => isCrouching;
    public bool IsSprinting =>
        Input.GetKey(KeyCode.LeftShift) &&
        !isCrouching &&
        IsMoving;
}
