using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class CentrifugeController : MonoBehaviour
{
    [Header("Centrifuge Settings")]
    public bool balanced = true;
    public bool isRotating = true;
    public float rotationSpeed = 200f;
    public float shakeIntensity = 0.1f;
    public float shakeFrequency = 10f;

    [Header("Tube Placement")]
    public Transform tubePlace1;
    public Transform tubePlace2;
    public Transform tubePlace7;

    public UnityEvent yellowTubeAdded;
    public UnityEvent purpleTubeAdded;

    [Header("Instruction System")]
    public FloatingInstructions instructions;

    private float savedRotationSpeed;
    private float savedShakeIntensity;
    private float savedShakeFrequency;

    private Vector3 centerOfBounds;
    private bool wasUnbalanced = false;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        centerOfBounds = CalculateCenterOfBounds();
        SaveRotationSettings();
    }

    private void Update()
    {
        HandleRotationToggle();

        if (!isRotating) return;

        if (balanced)
        {
            if (wasUnbalanced)
                ResetCentrifuge();
            RotateCentrifuge();
        }
        else
        {
            RotateCentrifuge();
            ApplyShaking();
        }

        wasUnbalanced = !balanced;
    }

    private void HandleRotationToggle()
    {
        if (!isRotating)
        {
            if (rotationSpeed != 0 || shakeIntensity != 0 || shakeFrequency != 0)
            {
                SaveRotationSettings();
                rotationSpeed = 0;
                shakeIntensity = 0;
                shakeFrequency = 0;
            }
        }
        else
        {
            if (rotationSpeed == 0 && shakeIntensity == 0 && shakeFrequency == 0)
            {
                RestoreRotationSettings();
            }
        }
    }

    private void RotateCentrifuge()
    {
        transform.RotateAround(centerOfBounds, Vector3.up, rotationSpeed * Time.deltaTime);
    }

    private void ApplyShaking()
    {
        float offsetX = Mathf.Sin(Time.time * shakeFrequency) * shakeIntensity;
        float offsetZ = Mathf.Cos(Time.time * shakeFrequency) * shakeIntensity;
        transform.position += new Vector3(offsetX, 0, offsetZ);
    }

    private void ResetCentrifuge()
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        centerOfBounds = CalculateCenterOfBounds();
    }

    private void SaveRotationSettings()
    {
        savedRotationSpeed = rotationSpeed;
        savedShakeIntensity = shakeIntensity;
        savedShakeFrequency = shakeFrequency;
    }

    private void RestoreRotationSettings()
    {
        rotationSpeed = savedRotationSpeed;
        shakeIntensity = savedShakeIntensity;
        shakeFrequency = savedShakeFrequency;
    }

    private Vector3 CalculateCenterOfBounds()
    {
        Bounds totalBounds = new Bounds(Vector3.zero, Vector3.zero);
        bool hasInitializedBounds = false;

        foreach (Transform child in transform)
        {
            MeshRenderer meshRenderer = child.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                if (!hasInitializedBounds)
                {
                    totalBounds = meshRenderer.bounds;
                    hasInitializedBounds = true;
                }
                else
                {
                    totalBounds.Encapsulate(meshRenderer.bounds);
                }
            }
        }

        return hasInitializedBounds ? totalBounds.center : transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (instructions == null) return;

        GameObject obj = collision.gameObject;

        Debug.Log(obj.name.Contains("YellowTube") + " " + instructions.IsStepCompleted("BalanceWeight") + " " + !instructions.IsStepCompleted("ThirdTube"));

        if (obj.name.Contains("YellowTube"))
        {
            yellowTubeAdded?.Invoke(); // Assigned in Inspector to call NotifyStepCompletedFromEvent("ThirdTube")
            Destroy(obj);
        }
        else if (obj.name.Contains("PurpleTube"))
        {
            purpleTubeAdded?.Invoke(); // Assigned to NotifyStepCompletedFromEvent("WaterTube")
            Destroy(obj);
        }
        else if (obj.name.Contains("YellowTube") || obj.name.Contains("PurpleTube"))
        {
            Destroy(obj);
        }
    }

    public void BalanceCentrifuge()
    {
        balanced = true;
        if (tubePlace1 != null) tubePlace1.gameObject.SetActive(true);
        if (tubePlace7 != null) tubePlace7.gameObject.SetActive(false);
    }

    public void StartCentrifuge()
    {
        isRotating = true;
    }

    public void StopCentrifuge()
    {
        isRotating = false;
    }
}
