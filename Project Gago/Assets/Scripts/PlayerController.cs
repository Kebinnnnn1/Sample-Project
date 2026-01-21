// Assets/Scripts/PlayerController.cs
using UnityEngine;

/// <summary>
/// Smooth FPS-style player controller with safe camera positioning.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float crouchSpeed = 3f;
    public float jumpHeight = 1.5f;
    public float gravity = -12f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 100f;
    public Transform cameraTransform;

    [Header("View Bob")]
    public float bobFrequency = 2f;
    public float bobAmplitude = 0.05f;

    [Header("Camera Tilt")]
    public float strafeTilt = 8f;
    public float tiltSmooth = 10f;

    [Header("Crouch")]
    public float crouchHeight = 1.0f;
    public float crouchCameraOffset = -0.6f;
    public float crouchTransitionSpeed = 10f;

    private CharacterController controller;
    private float yVelocity;
    private float xRotation;
    private float bobTimer;
    private float defaultHeight;
    private float currentTilt;

    private Vector3 cameraDefaultLocalPos;
    private float crouchLerp;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        defaultHeight = controller.height;
        cameraDefaultLocalPos = cameraTransform.localPosition;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
        HandleCrouch();
        HandleCameraEffects();
    }

    void HandleMovement()
    {
        bool grounded = controller.isGrounded;

        if (grounded && yVelocity < 0f)
            yVelocity = -2f;

        float speed = crouchLerp > 0.1f ? crouchSpeed : moveSpeed;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        if (Input.GetButtonDown("Jump") && grounded && crouchLerp < 0.1f)
            yVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

        yVelocity += gravity * Time.deltaTime;

        controller.Move((move * speed + Vector3.up * yVelocity) * Time.deltaTime);
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, currentTilt);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleCrouch()
    {
        bool crouching = Input.GetKey(KeyCode.LeftControl);

        float targetHeight = crouching ? crouchHeight : defaultHeight;
        controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * crouchTransitionSpeed);
        controller.center = Vector3.up * controller.height * 0.5f;

        crouchLerp = Mathf.Lerp(
            crouchLerp,
            crouching ? 1f : 0f,
            Time.deltaTime * crouchTransitionSpeed
        );
    }

    void HandleCameraEffects()
    {
        float moveAmount = new Vector3(controller.velocity.x, 0, controller.velocity.z).magnitude;

        // View bob
        float bobOffset = 0f;
        if (controller.isGrounded && moveAmount > 0.1f)
        {
            bobTimer += Time.deltaTime * bobFrequency;
            bobOffset = Mathf.Sin(bobTimer) * bobAmplitude;
        }
        else
        {
            bobTimer = 0f;
        }

        // Strafe tilt
        float inputX = Input.GetAxis("Horizontal");
        float targetTilt = -inputX * strafeTilt;
        currentTilt = Mathf.Lerp(currentTilt, targetTilt, Time.deltaTime * tiltSmooth);

        // Final camera position (SAFE)
        Vector3 targetCamPos = cameraDefaultLocalPos;
        targetCamPos.y += Mathf.Lerp(0f, crouchCameraOffset, crouchLerp);
        targetCamPos.y += bobOffset;

        cameraTransform.localPosition = Vector3.Lerp(
            cameraTransform.localPosition,
            targetCamPos,
            Time.deltaTime * 10f
        );
    }
}
