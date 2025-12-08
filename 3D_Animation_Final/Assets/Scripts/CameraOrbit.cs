using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public Transform target; // Assign the turbine's parent here
    public float orbitSpeed = 10f; // Degrees per second
    public float distance = 5f; // Distance from the target
    public float tiltAngle = 30f; // Downward tilt in degrees

    private float currentAngle = 0f;
     
    void LateUpdate()
    {
        if (target == null) return;

        // Increment the orbit angle
        currentAngle += orbitSpeed * Time.deltaTime;

        // Convert angle to radians
        float radians = currentAngle * Mathf.Deg2Rad;

        // Calculate new position around the target
        Vector3 offset = new Vector3(
            Mathf.Cos(radians) * distance,
            Mathf.Sin(tiltAngle * Mathf.Deg2Rad) * distance,
            Mathf.Sin(radians) * distance
        );

        transform.position = target.position + offset;

        // Always look at the target
        transform.LookAt(target);

        // Lock the X rotation (tilt)
        Vector3 euler = transform.eulerAngles;
        euler.x = tiltAngle;
        transform.rotation = Quaternion.Euler(euler);
    }
}
