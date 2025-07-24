using UnityEngine;

public class DroneFlight : MonoBehaviour
{
    public float avoidanceDistance = 10f;
    public float avoidanceStrength = 1f;
    public Vector3 TargetPosition;
    public float speed = 10f;
    public float arrivalDistance = 0.5f;

    void Update()
    {
        Vector3 direction = TargetPosition - transform.position;
        float distance = direction.magnitude;

        if (distance > arrivalDistance)
        {
            Vector3 avoidance = GetSteeringDirection(TargetPosition);
            direction.Normalize();

            Vector3 moveDir = direction + avoidance;
            if (moveDir != Vector3.zero)
                moveDir = moveDir.normalized;

            Vector3 movement = moveDir * speed * Time.deltaTime;
            transform.position += movement;

            transform.rotation = Quaternion.LookRotation(moveDir);
        }
        else
        {
            Debug.Log("Drone has arrived at the target position.");
        }
    }

    Vector3 GetSteeringDirection(Vector3 targetPosition)
    {
        Vector3 avoidance = Vector3.zero;
        int obstacleCount = 0;
        RaycastHit hit;

        
        Vector3[] directions = new Vector3[]
        {
            transform.forward,
            -transform.forward,
            transform.right,
            -transform.right,
            transform.up,
            -transform.up,
            (transform.forward + transform.right).normalized,
            (transform.forward - transform.right).normalized,
            (transform.forward + transform.up).normalized,
            (transform.forward - transform.up).normalized,
            (transform.right + transform.up).normalized,
            (-transform.right + transform.up).normalized,
            (transform.right - transform.up).normalized,
            (-transform.right - transform.up).normalized
        };

        foreach (Vector3 dir in directions)
        {
            // Debug.DrawRay(transform.position, dir * avoidanceDistance, Color.red);

            if (Physics.Raycast(transform.position, dir, out hit, avoidanceDistance))
            {
                if (hit.collider.CompareTag("Obstacle"))
                {
                    
                    Vector3 awayFromObstacle = transform.position - hit.point;
                    avoidance += awayFromObstacle.normalized / (hit.distance + 0.1f); 
                    obstacleCount++;
                }
            }
        }

        Vector3 finalAvoidance = obstacleCount > 0 ? avoidance.normalized * avoidanceStrength : Vector3.zero;
        Vector3 toTarget = (targetPosition - transform.position).normalized;

       
        float goalWeight = 0.7f;
        float avoidanceWeight = 0.3f;
        Vector3 blendedDirection = (toTarget * goalWeight + finalAvoidance * avoidanceWeight).normalized;

        return blendedDirection;
    }
}
