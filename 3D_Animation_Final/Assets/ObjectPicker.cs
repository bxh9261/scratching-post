using UnityEngine;

public class ObjectPicker : MonoBehaviour
{
    public float maxRaycastDistance = 5.0f; // Maximum distance for the raycast
    public LayerMask pickuppableLayer; // Layer for pickuppable objects

    private Pickuppable currentPickuppable;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            TryPickUpObject();
        }

        if (Input.GetMouseButtonDown(1)) // Right mouse button
        {
            DropObject();
        }

        if (currentPickuppable != null && currentPickuppable.isLocked)
        {
            DropObject();
        }
    }

    private void TryPickUpObject()
    {

        if (currentPickuppable != null) 
        {
            Debug.Log("You're currently holding " + currentPickuppable.gameObject.name);
            return; // Already holding an object
        }
        // Cast a ray directly forward from the camera
        Vector3 rayOrigin = Camera.main.transform.position;
        Vector3 rayDirection = Camera.main.transform.forward;

        // Debug line to visualize the raycast
        Debug.DrawRay(rayOrigin, rayDirection * maxRaycastDistance, Color.red, 1.0f);

        // Perform the raycast
        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, maxRaycastDistance, pickuppableLayer))
        {
            Pickuppable pickuppable = hit.collider.GetComponent<Pickuppable>();
            if (pickuppable != null)
            {
                currentPickuppable = pickuppable;
                pickuppable.PickUp(Camera.main.transform);
                Debug.Log($"Picked up {pickuppable.name}");
            }
        }
    }

    private void DropObject()
    {
        if (currentPickuppable != null)
        {
            currentPickuppable.Drop();
            Debug.Log($"Dropped {currentPickuppable.name}");
            currentPickuppable = null;
        }
    }
}
