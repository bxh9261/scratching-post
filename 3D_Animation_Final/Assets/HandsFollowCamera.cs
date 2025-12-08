using UnityEngine;

public class HandsFollowCamera : MonoBehaviour
{
    public Transform cameraTransform; // Reference to the camera
    public Transform leftHand; // Reference to the left hand
    public Transform rightHand; // Reference to the right hand
    public MonoBehaviour cameraController; // Reference to the camera movement script (e.g., CameraController)
    public MonoBehaviour playerMovement; // Reference to the player movement script (e.g., PlayerMovement)

    [Header("General Settings")]
    [Range(0, 90)] public float angleOffset = 25.0f; // Angle offset for relaxed hand positioning
    public float initialDistanceFromCamera = 1.0f; // Distance from the camera to the hands

    [Header("Hand Washing Settings")]
    public float washingDistance = 0.1f; // Minimum distance between hands during washing
    public float washingDuration = 10f; // Duration of washing animation
    public float alternateSpeed = 2f; // Speed of alternating movement
    public float oscillationDistance = 0.05f; // Distance for forward/backward oscillation
    public float sinkInteractionDistance = 1.0f; // Maximum distance to the Sink to initiate handwashing
    public bool cleanHands = false;

    private bool isWashing = false; // Whether the hands are currently washing
    private float washingTimer = 0f; // Timer for the washing animation
    private Vector3 leftHandInitialLocalOffset;
    private Vector3 rightHandInitialLocalOffset;
    private Rigidbody cameraRigidbody; // Rigidbody of the camera for freezing rotation

    private void Start()
    {
        if (cameraTransform == null)
        {
            Debug.LogWarning("Camera Transform is not assigned.");
            return;
        }

        if (leftHand == null || rightHand == null)
        {
            Debug.LogWarning("Left or Right Hand Transform is not assigned.");
            return;
        }

        // Try to get the Rigidbody from the camera
        cameraRigidbody = cameraTransform.GetComponent<Rigidbody>();

        // Store the hands' initial local offsets relative to the camera
        leftHandInitialLocalOffset = cameraTransform.InverseTransformPoint(leftHand.position);
        rightHandInitialLocalOffset = cameraTransform.InverseTransformPoint(rightHand.position);
    }

    private void Update()
    {
        // Check if the user clicks on the Sink
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Debug line for the raycast
            //Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 2.0f);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.Log($"Raycast hit: {hit.collider.gameObject.name}");

                if (hit.collider.gameObject.name == "Sink") // Check if the clicked object is the Sink
                {
                    // Check if the camera is within interaction distance of the Sink
                    float distanceToSink = Vector3.Distance(cameraTransform.position, hit.collider.transform.position);
                    Debug.Log($"Distance to Sink: {distanceToSink}");

                    if (distanceToSink <= sinkInteractionDistance)
                    {
                        StartHandWashing();
                    }
                }
            }
        }

        if (isWashing)
        {
            PerformHandWashing();
        }
        else
        {
            UpdateHandPositionsRelativeToCamera();
        }
    }

    private void StartHandWashing()
    {
        if (!isWashing)
        {
            isWashing = true;
            washingTimer = 0f; // Reset the washing timer

            // Lock the camera by disabling its movement script
            if (cameraController != null)
            {
                cameraController.enabled = false;
            }

            // Lock player movement by disabling its movement script
            if (playerMovement != null)
            {
                playerMovement.enabled = false;
            }

            // Freeze camera Rigidbody rotation
            if (cameraRigidbody != null)
            {
                cameraRigidbody.angularVelocity = Vector3.zero; // Reset any existing angular velocity
                cameraRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            }
            else;
            {
                Debug.Log("Can't find player's Rigidbody!");
            }
        }
    }

    private void UpdateHandPositionsRelativeToCamera()
    {
        // Get the camera's forward and up directions
        Vector3 forward = cameraTransform.forward;
        Vector3 up = cameraTransform.up;

        // Convert the angle offset to radians
        float angleOffsetRadians = Mathf.Deg2Rad * angleOffset;

        // Calculate the direction for the hands' positions
        Vector3 offsetDirection = Mathf.Cos(angleOffsetRadians) * forward -
                                  Mathf.Sin(angleOffsetRadians) * up;

        // Calculate the base position relative to the camera
        Vector3 basePosition = cameraTransform.position + offsetDirection.normalized * initialDistanceFromCamera;

        // Update the left and right hands' positions based on their initial offsets
        leftHand.position = cameraTransform.TransformPoint(leftHandInitialLocalOffset);
        rightHand.position = cameraTransform.TransformPoint(rightHandInitialLocalOffset);

        // Ensure the hands always face away from the camera
        Quaternion handRotation = Quaternion.LookRotation(offsetDirection, Vector3.up);
        leftHand.rotation = handRotation;
        rightHand.rotation = handRotation;
    }

    private void PerformHandWashing()
    {
        // Calculate the midpoint between the two hands
        Vector3 middlePoint = (leftHand.position + rightHand.position) / 2;

        // Calculate the direction between the two hands
        Vector3 direction = (rightHand.position - leftHand.position).normalized;

        // Adjust the target positions to keep the hands slightly apart
        Vector3 leftTarget = middlePoint - direction * (washingDistance / 2);
        Vector3 rightTarget = middlePoint + direction * (washingDistance / 2);

        // Gradually move the hands toward their target positions
        leftHand.position = Vector3.MoveTowards(leftHand.position, leftTarget, Time.deltaTime);
        rightHand.position = Vector3.MoveTowards(rightHand.position, rightTarget, Time.deltaTime);

        // Add oscillation (forward/backward movement)
        float offset = Mathf.Sin(Time.time * alternateSpeed) * oscillationDistance;
        leftHand.position += leftHand.forward * offset * Time.deltaTime;
        rightHand.position += rightHand.forward * -offset * Time.deltaTime;

        // Increment the timer
        washingTimer += Time.deltaTime;

        // Stop washing animation after the duration
        if (washingTimer >= washingDuration)
        {
            isWashing = false;
            cleanHands = true;

            // Unlock the camera by enabling its movement script
            if (cameraController != null)
            {
                cameraController.enabled = true;
            }
            // Unlock player movement by enabling its movement script
            if (playerMovement != null)
            {
                playerMovement.enabled = true;
            }

            // Unfreeze camera Rigidbody rotation
            if (cameraRigidbody != null)
            {
                cameraRigidbody.constraints = RigidbodyConstraints.None;
            }
        }
    }
}


