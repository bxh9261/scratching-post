using UnityEngine;

public class HandWashingAnimation : MonoBehaviour
{
    public Transform leftHand; // Reference to the left hand
    public Transform rightHand; // Reference to the right hand
    public float washingDistance = 0.1f; // Minimum distance between the hands while washing
    public float washingDuration = 10f; // Duration of the washing animation in seconds
    public float alternateSpeed = 2f; // Speed of the alternating movement
    public KeyCode startKey = KeyCode.X; // Key to start the hand washing animation

    private Vector3 leftHandOriginalPosition;
    private Vector3 rightHandOriginalPosition;

    private bool isWashing = false;
    private float washingTimer = 0f;

    private void Start()
    {
        // Store the original positions of the hands
        if (leftHand != null) leftHandOriginalPosition = leftHand.position;
        if (rightHand != null) rightHandOriginalPosition = rightHand.position;
    }

    private void Update()
    {
        // Start the hand washing animation when the specified key is pressed
        if (Input.GetKeyDown(startKey) && !isWashing)
        {
            isWashing = true;
            washingTimer = 0f; // Reset the timer
        }

        // Perform the animation if washing
        if (isWashing)
        {
            PerformHandWashing();
        }
    }

    private void PerformHandWashing()
    {
        // Calculate the shortest line between the two hands
        Vector3 middlePoint = (leftHand.position + rightHand.position) / 2;

        // Gradually move the hands toward the middle
        leftHand.position = Vector3.MoveTowards(leftHand.position, middlePoint - Vector3.right * washingDistance / 2, Time.deltaTime);
        rightHand.position = Vector3.MoveTowards(rightHand.position, middlePoint + Vector3.right * washingDistance / 2, Time.deltaTime);

        // Alternate movement if they are close enough
        if (Vector3.Distance(leftHand.position, rightHand.position) <= washingDistance * 2)
        {
            // Alternate the forward and backward movement
            float offset = Mathf.Sin(Time.time * alternateSpeed) * washingDistance / 2;
            leftHand.position += leftHand.forward * offset * Time.deltaTime;
            rightHand.position -= rightHand.forward * offset * Time.deltaTime;

            // Increment the timer
            washingTimer += Time.deltaTime;

            // Stop the animation after the duration ends
            if (washingTimer >= washingDuration)
            {
                ResetHandPositions();
            }
        }
    }

    private void ResetHandPositions()
    {
        // Move hands back to their original positions
        leftHand.position = leftHandOriginalPosition;
        rightHand.position = rightHandOriginalPosition;

        // Reset washing state
        isWashing = false;
    }
}
