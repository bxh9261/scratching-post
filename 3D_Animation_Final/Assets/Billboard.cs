using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform cam;

    void Start()
    {
        cam = Camera.main.transform;
    }

    void LateUpdate()
    {
        // Face the camera, but keep the cat upright
        Vector3 lookDir = transform.position - cam.position;
        lookDir.y = 0; // Prevent tilting
        transform.rotation = Quaternion.LookRotation(lookDir);
    }
}