using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class Pickuppable : MonoBehaviour
{
    private GameManager gameManager;

    private Transform cameraTransform; // Reference to the camera
    private bool isPickedUp = false; // Whether the object is currently picked up
    public bool isLocked = false; // Whether the object is locked in place
    private float distanceToCamera; // Distance from the object to the camera
    private Vector3 initialOffset; // Initial offset from the camera when picked up

    public enum ContentType { Empty, Acid, Water, Biohazard, Bleach }

    [Header("Contents Settings")]
    public ContentType contents = ContentType.Empty; // What liquid, if any, is inside the object

    [Header("Lock Settings")]
    public GameObject lockTarget; // GameObject that locks this Pickuppable upon collision
    public Vector3 lockPositionOffset; // Offset for the locked position relative to the lock target
    public bool snapToLockRotation = true; // Whether the object should match the lock target's rotation
    public UnityEvent onObjectLocked;

    [Header("Fragility Settings")]
    public bool isFragile = false; // Whether the object can break when dropped
    public GameObject floor; // Assign the floor object in the Inspector
    public GameObject brokenGlassPrefab; // Prefab to replace this object when broken
    public GameObject acidSpillPrefab; // Prefab for an acid spill
    public GameObject waterSpillPrefab; // Prefab for a water spill
    public GameObject biohazardSpillPrefab; // Prefab for biohazard spill
    public GameObject disinfectedSpillPrefab; // Prefab for neutralized biohazard spill


    private void Start()
    {
        cameraTransform = Camera.main.transform;
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        if (isPickedUp && !isLocked)
        {
            // Limit the maximum pickup distance to 1 unit
            distanceToCamera = Mathf.Min(distanceToCamera, 1f);

            // Keep the object at the same distance and angle from the camera
            Vector3 newPosition = cameraTransform.position + cameraTransform.forward * distanceToCamera + initialOffset;
            transform.position = newPosition;

            // Keep the object facing the camera
            transform.rotation = Quaternion.LookRotation(transform.position - cameraTransform.position, Vector3.up);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isLocked)
        {
            Debug.Log($"{gameObject.name} is already locked in place.");
            return;
        }

        // Check if the object should lock in place
        if (collision.gameObject == lockTarget)
        {
            Debug.Log($"{collision.gameObject.name} collided with the lock target.");
            LockInPlace();
        }

        // Handle fragile objects breaking when colliding with the floor
        if (isFragile && collision.gameObject == floor)
        {
            Debug.Log($"{gameObject.name} is fragile and hit the floor. Breaking...");
            BreakObject();
        }
    }

    private void LockInPlace()
    {
        isLocked = true;
        isPickedUp = false;

        // Set position and rotation
        if (lockTarget != null)
        {
            transform.position = lockTarget.transform.position + lockPositionOffset;

            if (snapToLockRotation)
            {
                transform.rotation = lockTarget.transform.rotation;
            }
        }

        // Optionally, disable the object's Rigidbody to make it static
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        Debug.Log($"{gameObject.name} has been locked into position.");
        onObjectLocked?.Invoke();
    }

    public bool IsObjectLockedOnTarget()
    {
        return isLocked;
    }

    public void PickUp(Transform camera)
    {
        Debug.Log($"{gameObject.name}: PickUp called. isLocked = {isLocked}");
        if (isLocked) return;

        isPickedUp = true;
        cameraTransform = camera;

        // Calculate the initial distance and offset
        distanceToCamera = Vector3.Distance(cameraTransform.position, transform.position);
        distanceToCamera = Mathf.Min(distanceToCamera, 1f); // Ensure max distance is 1
        initialOffset = transform.position - (cameraTransform.position + cameraTransform.forward * distanceToCamera);
    }

    public void Drop()
    {
        if (isLocked) return;

        isPickedUp = false;
    }

    private void BreakObject()
    {
        if (brokenGlassPrefab != null)
        {
            Vector3 brokenGlassOnFloorPosition = new Vector3(transform.position.x, 0, transform.position.z);
            Instantiate(brokenGlassPrefab, brokenGlassOnFloorPosition, Quaternion.identity);
        }

        // Instantiate a spill if applicable
        SpawnSpill();

        // Run checks only if GameManager exists
        if (gameManager != null)
        {
            //gameManager.CheckBrokenGlass();
            //gameManager.CheckAcidSpills();
        }

        // Destroy the original object
        Destroy(gameObject);
    }


    private void SpawnSpill()
    {
        GameObject spillPrefab = null;

        switch (contents)
        {
            case ContentType.Acid:
                spillPrefab = acidSpillPrefab;
                break;
            case ContentType.Water:
                spillPrefab = waterSpillPrefab;
                break;
            case ContentType.Biohazard:
                spillPrefab = biohazardSpillPrefab;
                break;
            case ContentType.Empty:
            case ContentType.Bleach:
                return; // No spill if empty or bleach
        }

        if (spillPrefab != null)
        {
            Vector3 spillPosition = new Vector3(transform.position.x, 0, transform.position.z);
            Instantiate(spillPrefab, spillPosition, Quaternion.identity);
            Debug.Log($"Spill of {contents} created at {spillPosition}");
        }
    }
}
