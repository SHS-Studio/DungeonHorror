using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform target; // Target to follow
    private NavMeshAgent agent;
    public bool isActivated = false;
    public bool isAvoidingLight = false;

    public float normalSpeed = 5f;
    public float slowSpeed = 2f;
    public float avoidLightTime = 2f; // Time spent avoiding light
    public float avoidDistance = 5f; // How far enemy moves to avoid

    public Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        target = EnemyManager.instance.Target;
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
            agent.SetDestination(target.position);
            agent.speed = normalSpeed;
           // animator.SetBool("Idel", false);
            animator.SetTrigger("Run");
        }
    }

    public void CheckSpotlight()
    {
        if (!isActivated)
        {
            isActivated = true; // Activate enemy
            animator.SetTrigger("Idel");
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
        animator.SetTrigger("Walk");
        Vector3 avoidDirection = FindNewPath();

        if (avoidDirection != Vector3.zero) // Ensure we found a valid point
        {
            agent.SetDestination(avoidDirection);
            animator.SetTrigger("Walk");
            Debug.Log("Avoiding light: Moving to " + avoidDirection);
        }
        else
        {
            Debug.LogWarning("No valid avoidance position found, staying in place!");
        }

        Invoke(nameof(ResumeChase), avoidLightTime); // Resume normal chase after time
    }

    Vector3 FindNewPath()
    {
        for (int i = 0; i < 10; i++) // Try multiple times to find a valid position
        {
            Vector3 randomDirection = Random.insideUnitSphere * avoidDistance;
            randomDirection += transform.position;

            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit navHit, avoidDistance, NavMesh.AllAreas))
            {
                return navHit.position; // Found a valid position
            }
        }

        return Vector3.zero; // No valid position found
    }

    public void ResumeChase()
    {
        isAvoidingLight = false;
        if (target != null)
        {
            agent.SetDestination(target.position);
        }
        agent.speed = normalSpeed;
        animator.SetTrigger("Run");
        Debug.Log("Resuming chase.");
    }
}