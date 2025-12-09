using UnityEngine;

/// <summary>
/// Smoothed free camera / freecam controller using Unity's old Input system (Input.GetAxis / Input.GetKey).
/// - WASD / Arrow Keys for horizontal movement ("Horizontal" / "Vertical" axes in Input Manager)
/// - Hold Left Shift to sprint
/// - Q / E for down / up
/// - Right Mouse Button to enable mouse-look (locks cursor while held)
/// - Smooths both rotation and translation with configurable smoothing times
/// Drop this script on your Camera (or a parent GameObject) and tweak public fields.
/// </summary>
[DisallowMultipleComponent]
public class SmoothedFreeCam : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;                 // base walk speed (units / sec)
    public float sprintMultiplier = 2.0f;        // multiplier when holding sprint
    public float movementSmoothTime = 0.08f;     // smoothing time for movement velocity
    public float acceleration = 10f;             // responsiveness when increasing speed
    public float deceleration = 10f;             // responsiveness when stopping

    [Header("Vertical Movement")]
    public KeyCode ascendKey = KeyCode.E;        // move up
    public KeyCode descendKey = KeyCode.Q;       // move down
    public bool allowVerticalMovement = true;

    [Header("Rotation")]
    public float rotationSensitivity = 6.0f;     // mouse sensitivity multiplier
    public float rotationSmoothTime = 0.03f;     // smoothing time for rotation
    public float minPitch = -89f;                // look down limit
    public float maxPitch = 89f;                 // look up limit

    [Header("Cursor & Input")]
    public KeyCode toggleCursorKey = KeyCode.Mouse1; // hold to enable mouse look (default: Right Mouse Button)
    public bool lockCursorWhileLooking = true;

    [Header("Optional")]
    public bool usePhysicsForMovement = false;   // if true, expects a Rigidbody on same GameObject and moves via MovePosition
    public Rigidbody attachedRigidbody;          // optional: assign a Rigidbody if using physics mode

    // Internal state
    Vector3 currentVelocity = Vector3.zero;      // used by SmoothDamp to smooth movement
    Vector3 velocityVelocity = Vector3.zero;     // additional velocity smoothing

    Vector2 smoothMouseVelocity = Vector2.zero;  // used by SmoothDamp for mouse
    Vector2 currentMouseDelta = Vector2.zero;    // smoothed mouse delta

    float yaw;   // rotation around Y (left/right)
    float pitch; // rotation around X (up/down)

    void Start()
    {
        Vector3 euler = transform.eulerAngles;
        yaw = euler.y;
        pitch = euler.x;

        if (usePhysicsForMovement && attachedRigidbody == null)
        {
            attachedRigidbody = GetComponent<Rigidbody>();
            if (attachedRigidbody == null)
            {
                Debug.LogWarning("SmoothedFreeCam: usePhysicsForMovement is true but no Rigidbody found. Falling back to non-physics movement.");
                usePhysicsForMovement = false;
            }
            else
            {
                // make sure interpolation is enabled for smooth movement
                attachedRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            }
        }

        // Optional: lock cursor initially? We'll lock only while RMB held by default
        if (lockCursorWhileLooking)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    void Update()
    {
        HandleRotation();
        HandleMovement();
    }

    void HandleRotation()
    {
        bool lookActive = Input.GetKey(toggleCursorKey);

        if (lockCursorWhileLooking)
        {
            Cursor.visible = !lookActive;
            Cursor.lockState = lookActive ? CursorLockMode.Locked : CursorLockMode.None;
        }

        if (!lookActive)
            return; // only rotate while the toggle key is held

        // Old Input system: "Mouse X" and "Mouse Y".
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Raw scaled by sensitivity (we'll smooth the scaled values)
        Vector2 targetMouseDelta = new Vector2(mouseX * rotationSensitivity, mouseY * rotationSensitivity);

        // Smooth the mouse delta (so sudden mouse moves are eased)
        float smoothTime = Mathf.Max(0.0001f, rotationSmoothTime);
        currentMouseDelta.x = Mathf.SmoothDamp(currentMouseDelta.x, targetMouseDelta.x, ref smoothMouseVelocity.x, smoothTime);
        currentMouseDelta.y = Mathf.SmoothDamp(currentMouseDelta.y, targetMouseDelta.y, ref smoothMouseVelocity.y, smoothTime);

        // integrate rotation
        yaw += currentMouseDelta.x;
        pitch -= currentMouseDelta.y; // invert Y for typical FPS-style look

        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    void HandleMovement()
    {
        // Get input axes (old Input system). "Horizontal" (A/D or Left/Right) and "Vertical" (W/S or Up/Down)
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");

        // Build local movement vector
        Vector3 localInput = new Vector3(inputX, 0f, inputZ);

        // Vertical movement via keys (E up, Q down) if enabled
        if (allowVerticalMovement)
        {
            if (Input.GetKey(ascendKey)) localInput.y += 1f;
            if (Input.GetKey(descendKey)) localInput.y -= 1f;
        }

        // normalize diagonal movement so speed remains consistent
        localInput = Vector3.ClampMagnitude(localInput, 1f);

        // Determine target speed (sprint)
        float targetSpeed = moveSpeed * (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? sprintMultiplier : 1f);

        // desired world-space velocity relative to camera orientation
        Vector3 desiredVelocity = transform.TransformDirection(localInput) * targetSpeed;

        // Smooth velocity using SmoothDamp for inertia
        // We'll use different responsiveness when accelerating vs decelerating
        float speedDifference = (desiredVelocity - currentVelocity).magnitude;
        float useSmoothTime = movementSmoothTime;

        // If we're trying to increase speed (accelerate) use smaller smooth time (faster response)
        if (Vector3.Dot(desiredVelocity, currentVelocity) > 0f && desiredVelocity.magnitude > currentVelocity.magnitude)
            useSmoothTime = Mathf.Max(0.0001f, movementSmoothTime / Mathf.Clamp(acceleration, 0.0001f, 100f));
        else
            useSmoothTime = Mathf.Max(0.0001f, movementSmoothTime * Mathf.Clamp(deceleration, 0.0001f, 100f));

        currentVelocity = Vector3.SmoothDamp(currentVelocity, desiredVelocity, ref velocityVelocity, useSmoothTime);

        // Apply movement
        if (usePhysicsForMovement && attachedRigidbody != null)
        {
            // Move via Rigidbody for physics-friendly movement
            Vector3 newPos = attachedRigidbody.position + currentVelocity * Time.deltaTime;
            attachedRigidbody.MovePosition(newPos);
        }
        else
        {
            transform.position += currentVelocity * Time.deltaTime;
        }
    }

    // Optional: expose a public method to forcibly set camera orientation (useful for editor tools)
    public void SetRotation(float newPitch, float newYaw)
    {
        pitch = Mathf.Clamp(newPitch, minPitch, maxPitch);
        yaw = newYaw;
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}
