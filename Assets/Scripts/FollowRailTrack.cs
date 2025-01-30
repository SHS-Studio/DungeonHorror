using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowRailTrack : MonoBehaviour
{
    public static FollowRailTrack instance { get; set; }

    [Header("Path Settings")]
    public List<Transform> waypoints; // List of waypoints to follow
    public float speed = 5f;          // Movement speed
    public float reachThreshold = 0.1f; // Distance to waypoint to consider it reached

    public int currentWaypointIndex = 0; // Current waypoint being targeted

    [Header("Platform Settings")]
    public GameObject platform;       // Reference to the platform object
    public float platformLength = 10f; // Length to move the platform forward
  

    private Vector3 initialPlatformPosition;

    public void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    void Start()
    {
        if (waypoints == null || waypoints.Count == 0)
        {
            Debug.LogWarning("No initial waypoints assigned for the path.");
            return;
        }

        initialPlatformPosition = platform.transform.position;
    }

    void Update()
    {
        if (waypoints == null || waypoints.Count == 0)
        {
            Debug.LogWarning("No waypoints assigned for the path.");
            return;
        }

        MoveAlongPath();
    }

    void MoveAlongPath()
    {
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector3 direction = (targetWaypoint.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        float distanceToWaypoint = Vector3.Distance(transform.position, targetWaypoint.position);
        if (distanceToWaypoint <= reachThreshold)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Count)
            {
                currentWaypointIndex = 0; // Reset to the first waypoint
                MovePlatformForward();
               
            }
        }

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * speed);
        }
    }

    void MovePlatformForward()
    {
        // Move the platform forward
        Vector3 forwardOffset = new Vector3(0, 0, platformLength);
        platform.transform.position += forwardOffset;
        platform.transform.rotation = Quaternion.Euler(0,0,0);

        // Adjust waypoints positions to match the new platform position
        for (int i = 0; i < waypoints.Count; i++)
        {
            waypoints[i].position += forwardOffset;
        }
    }

    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Count == 0)
            return;

        Gizmos.color = Color.green;
        for (int i = 0; i < waypoints.Count; i++)
        {
            if (waypoints[i] != null)
            {
                Gizmos.DrawSphere(waypoints[i].position, 0.2f);
                if (i < waypoints.Count - 1 && waypoints[i + 1] != null)
                {
                    Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
                }
            }
        }
    }
}