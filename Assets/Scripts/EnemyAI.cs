using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform target; // Target to follow
    public Light playerSpotlight; // Player's spotlight
    private NavMeshAgent agent;
    public bool isActivated = false;
    public bool isAvoidingLight = false;

    public float normalSpeed = 5f;
    public float slowSpeed = 2f;
    public float avoidLightTime = 2f; // Time spent avoiding light
    public float avoidDistance = 5f; // How far enemy moves to avoid

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = normalSpeed; // Set initial speed

        if (target == null)
        {
            Debug.LogError("Target is not assigned in EnemyAI script!");
        }
    }

    void Update()
    {
        if (isActivated && !isAvoidingLight && target != null)
        {
            if (target != null)
            {
                agent.SetDestination(target.position);
            }
            agent.speed = normalSpeed;
        }
    }

    public void CheckSpotlight()
    {
        if (!isActivated)
        {
            isActivated = true; // Activate enemy
        }

        if (!isAvoidingLight)
        {
            isAvoidingLight = true;
            AvoidSpotlight();
        }
    }

    void AvoidSpotlight()
    {
        agent.speed = slowSpeed; // Slow down enemy while avoiding
        Vector3 avoidDirection = FindNewPath();

        if (NavMesh.SamplePosition(avoidDirection, out NavMeshHit navHit, avoidDistance, NavMesh.AllAreas))
        {
            agent.SetDestination(navHit.position);
        }

        Invoke(nameof(ResumeChase), avoidLightTime); // Resume normal chase after time
    }

    Vector3 FindNewPath()
    {
        Vector3 randomDirection = Random.insideUnitSphere * avoidDistance;
        randomDirection += transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit navHit, avoidDistance, NavMesh.AllAreas))
        {
            return navHit.position;
        }

        return navHit.position; // Stay in place if no valid path
    }

  public  void ResumeChase()
    {
        isAvoidingLight = false;
        if (target != null)
        {
            agent.SetDestination(target.position);
        }
        agent.speed = normalSpeed;
    }
}