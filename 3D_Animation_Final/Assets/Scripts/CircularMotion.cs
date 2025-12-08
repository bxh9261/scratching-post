using UnityEngine;

public class CircularMotion : MonoBehaviour
{
    public float diameter = 0.002f; // Diameter of the circle
    public float revolutionTime = 0.25f; // Time for one full revolution in seconds

    private float radius; // Radius of the circle (half the diameter)
    private float angularSpeed; // Angular speed (radians per second)
    private Vector3 startPosition; // Store initial position to maintain Z

    void Start()
    {
        radius = diameter / 2.0f;
        angularSpeed = 2 * Mathf.PI / revolutionTime; // Calculate angular speed
        startPosition = transform.position; // Store the initial position
    }

    void Update()
    {
        float time = Time.time; // Current time since the start of the game
        float angle = time * angularSpeed; // Angle based on time and speed

        // Calculate new x and y positions on the circle
        float newX = Mathf.Cos(angle) * radius;
        float newY = Mathf.Sin(angle) * radius;

        // Update the position on the XY-plane, keeping Z constant
        transform.position = new Vector3(startPosition.x + newX, startPosition.y, startPosition.z + newY);
    }
}

