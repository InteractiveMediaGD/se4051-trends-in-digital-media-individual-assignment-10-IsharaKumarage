// ============================================================
// PlayerMovement.cs
// Smart Relaxation Room — SE4051 Individual Assignment
// Author: Ishara Kumarage
//
// First-person WASD movement with mouse look.
// Attach this script to the Player GameObject.
// The Main Camera should be a child of the Player.
// ============================================================

using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // ── Inspector Settings ────────────────────────────────────────
    [Header("Movement Settings")]
    [Tooltip("How fast the player walks (units/second)")]
    public float moveSpeed = 4.0f;


    [Header("Mouse Look Settings")]
    [Tooltip("Mouse sensitivity for horizontal (Y-axis) rotation")]
    public float mouseSensitivityX = 2.0f;

    [Tooltip("Mouse sensitivity for vertical camera tilt")]
    public float mouseSensitivityY = 2.0f;

    [Tooltip("Maximum angle the camera can look up/down (degrees)")]
    public float verticalClampAngle = 80.0f;

    // ── Private references ────────────────────────────────────────
    private Camera playerCamera;         // Child camera
    private CharacterController cc;      // Unity CharacterController for collision
    private float verticalRotation = 0f; // Accumulated vertical tilt

    // ── UI toggle — set by Escape key ────────────────────────────
    private bool cursorLocked = true;

    // ── Unity lifecycle ──────────────────────────────────────────
    void Start()
    {
        // Find the child camera automatically
        playerCamera = GetComponentInChildren<Camera>();
        if (playerCamera == null)
            Debug.LogError("[PlayerMovement] No Camera found as child of Player!");

        // Get or add CharacterController
        cc = GetComponent<CharacterController>();
        if (cc == null)
        {
            cc = gameObject.AddComponent<CharacterController>();
            cc.height = 1.8f;
            cc.center = new Vector3(0, 0.9f, 0);
            cc.radius = 0.3f;
        }

        LockCursor(true);
    }

    void Update()
    {
        HandleCursorLock();
        HandleMouseLook();
        HandleMovement();
    }

    // ── Movement ─────────────────────────────────────────────────
    public bool isSitting = false;
    private Vector3 preSitPos;
    private Quaternion preSitRot;

    public void ToggleSit(Transform seatPoint)
    {
        isSitting = !isSitting;
        if (cc != null) cc.enabled = !isSitting;

        if (isSitting)
        {
            preSitPos = transform.position;
            preSitRot = transform.rotation;
            transform.position = seatPoint.position;
            transform.rotation = seatPoint.rotation;
            verticalRotation = 0f; // Reset look angle
        }
        else
        {
            transform.position = preSitPos;
            transform.rotation = preSitRot;
        }
    }

    private void HandleMovement()
    {
        if (isSitting) return;

        float horizontal = Input.GetAxisRaw("Horizontal"); // A / D
        float vertical   = Input.GetAxisRaw("Vertical");   // W / S

        // Build movement direction relative to player's facing
        Vector3 direction = transform.right * horizontal + transform.forward * vertical;
        direction = Vector3.ClampMagnitude(direction, 1f); // Prevent diagonal speed boost

        // Target velocity
        Vector3 targetVelocity = direction * moveSpeed;

        // Simple gravity
        targetVelocity.y = -9.81f * Time.deltaTime;

        // Apply smooth movement via CharacterController
        cc.Move(targetVelocity * Time.deltaTime);
    }

    // ── Mouse Look ───────────────────────────────────────────────
    private void HandleMouseLook()
    {
        if (!cursorLocked) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivityX;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivityY;

        // Rotate the player body left/right
        transform.Rotate(Vector3.up * mouseX);

        // Tilt the camera up/down (clamped)
        verticalRotation -= mouseY;
        verticalRotation  = Mathf.Clamp(verticalRotation, -verticalClampAngle, verticalClampAngle);

        if (playerCamera != null)
            playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    // ── Cursor control ────────────────────────────────────────────
    private void HandleCursorLock()
    {
        // Press Escape to release the cursor (so UI buttons can be clicked)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            LockCursor(!cursorLocked);
        }


    }

    private void LockCursor(bool locked)
    {
        cursorLocked           = locked;
        Cursor.lockState       = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible         = !locked;
    }

    // ── Public helper — UI can call this to temporarily unlock ───
    public void UnlockCursorForUI()
    {
        LockCursor(false);
    }
}
