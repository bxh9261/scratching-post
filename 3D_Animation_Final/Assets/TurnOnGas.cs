using UnityEngine;

public class TurnOnGas : MonoBehaviour
{
    public float rotationAngle = -90f; // Default to -90 degrees on Z-axis
    public float rotationSpeed = 5f; // Smooth rotation speed
    private bool isRotated = false; // Tracks rotation state

    private Vector3 meshCenter; // Stores the mesh center

    private void Start()
    {
        // Calculate the center of the mesh to rotate around
        meshCenter = GetMeshCenter();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) // Press 'E' to toggle (can change)
        {
            RotateGasValve();
        }
    }

    public void RotateGasValve()
    {
        // Define rotation direction
        float targetAngle = isRotated ? -rotationAngle : rotationAngle;

        // Rotate around the mesh center
        transform.RotateAround(meshCenter, transform.forward, targetAngle);

        isRotated = !isRotated;
    }

    private Vector3 GetMeshCenter()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();

        if (meshFilter == null || meshFilter.sharedMesh == null)
        {
            Debug.LogError("No MeshFilter found on this object! Make sure it has a MeshFilter component.");
            return transform.position; // Fallback to object position
        }

        // Get the local center of the mesh
        Bounds bounds = meshFilter.sharedMesh.bounds;
        Vector3 localCenter = bounds.center;

        // Convert to world position
        return transform.TransformPoint(localCenter);
    }
}
