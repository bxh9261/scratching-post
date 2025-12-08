using UnityEngine;

public class LabCoatManager : MonoBehaviour
{
    [Header("Required GameObjects")]
    public GameObject coatRack; // Assign the Coat Rack object in the Inspector
    public GameObject firstPersonController; // Assign the First Person Controller
    public Transform scientistParent; // Assign the scientist (grandchild of FPC)

    private Vector3 initialLocalPosition; // Initial position relative to FPC
    private Quaternion initialLocalRotation; // Initial rotation relative to FPC
    public bool isWorn = false; // Tracks if the coat is being worn
    private BoxCollider coatCollider; // Reference to BoxCollider

    private void Start()
    {
        if (coatRack == null || firstPersonController == null || scientistParent == null)
        {
            Debug.LogError("Ensure Coat Rack, FPC, and Scientist Parent are assigned!");
            return;
        }

        // Save the initial local position & rotation relative to the scientist parent
        initialLocalPosition = transform.localPosition;
        initialLocalRotation = transform.localRotation;

        // Get or add a Box Collider
        coatCollider = GetComponent<BoxCollider>();
        if (coatCollider == null)
        {
            coatCollider = gameObject.AddComponent<BoxCollider>();
        }

        // Move coat to coat rack at the beginning
        MoveToCoatRack();
    }

    private void MoveToCoatRack()
    {
        // Detach from FPC (move to world space first)
        transform.SetParent(null, true);

        // Move to Coat Rack's position & rotation
        transform.position = coatRack.transform.position + new Vector3(0,0.7f,-0.2f);
        transform.rotation = coatRack.transform.rotation;

        // Reset parenting so it's no longer a child of anything
        transform.SetParent(null);

        // Enable Box Collider for interaction
        if (coatCollider != null)
        {
            coatCollider.enabled = true;
        }

        isWorn = false;
    }

    private void MoveToPlayer()
    {
        if (scientistParent == null)
        {
            Debug.LogError("Scientist parent not assigned!");
            return;
        }

        // Set Lab Coat as a child of the correct Scientist object
        transform.SetParent(scientistParent, true);

        // Move it back to its initial saved position relative to the scientist
        transform.localPosition = initialLocalPosition;
        transform.localRotation = initialLocalRotation;

        // Disable Box Collider when worn
        if (coatCollider != null)
        {
            coatCollider.enabled = false;
        }

        isWorn = true;
    }

    private void OnMouseDown()
    {
        // When the lab coat is clicked, toggle between rack & player
        if (isWorn)
        {
            Debug.Log("Coat was clicked, but I think it's already being worn by you. If this isn't the case, something went wrong :(");
        }
        else
        {
            MoveToPlayer();
        }
    }
}
