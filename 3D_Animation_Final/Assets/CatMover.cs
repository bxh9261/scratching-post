using UnityEngine;
using System.Collections.Generic;

public class CatMover : MonoBehaviour
{
    [Header("Movement")]
    public List<Transform> locations;      // fill this in the Inspector
    public float moveSpeed = 2f;
    public float turnSpeed = 5f;

    private int currentTarget = 0;
    private bool isMoving = false;
    private Vector3 velocity;

    void Update()
    {
        if (isMoving)
            MoveTowardsTarget();
    }

    public void GoToLocation(int index)
    {
        if (index < 0 || index >= locations.Count) return;
        currentTarget = index;
        isMoving = true;
    }

    private void MoveTowardsTarget()
    {
        Vector3 targetPos = new Vector3(
            locations[currentTarget].position.x,
            transform.position.y,           // keep grounded
            locations[currentTarget].position.z
        );

        // Smooth motion
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            moveSpeed * Time.deltaTime
        );

        // Smooth turning
        Vector3 direction = (targetPos - transform.position).normalized;
        if (direction.magnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                targetRot,
                turnSpeed * Time.deltaTime
            );
        }

        // Arrived?
        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
        {
            isMoving = false;
        }
    }
}
