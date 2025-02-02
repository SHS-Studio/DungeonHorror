using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public enum EnemytYPE
{
    LightChaser,
    LightAvoider

}

public class EnemyAI : MonoBehaviour
{
    public EnemytYPE type;  
    public Transform target; // Target to follow
    private NavMeshAgent agent;
    public bool isActivated = false;

    public float normalSpeed = 5f;
    public float slowSpeed = 2f;

    public float avoidDistance = 5f; // How far enemy moves to avoid light

    public float attackRange = 1.5f; // Attack range
    public float attackCooldown = 1.5f; // Time between attacks

    public Animator animator;
    private float lastAttackTime;
    public bool IsAttacking = false;    



    void Start()
    {
        animator = GetComponent<Animator>();
        target = EnemyManager.instance.Target;
        agent = GetComponent<NavMeshAgent>();

        if (target == null)
        {
            Debug.LogError("Target is not assigned in EnemyAI script!");
        }
    }

    void Update()
    {
        if (isActivated)
        {
            switch (type)
            {
                case EnemytYPE.LightChaser:
                    if (!EnemyManager.instance.IsSpoted)
                    {
                        if (!IsAttacking)
                        {
                            FindingTarget();
                        }

                    }
                    else
                    {
                        if (!IsAttacking)
                        {
                            ChaseTarget();
                        }

                    }
                    break;
                case EnemytYPE.LightAvoider:
                    if (!EnemyManager.instance.IsSpoted)
                    {
                        if (!IsAttacking)
                        {
                            ChaseTarget();
                        }

                    }
                    else
                    {
                        if (!IsAttacking)
                        {
                            FindingTarget();
                        }

                    }

                    break;
            }

            Attack();
        }

    }

    public void CheckSpotlight()
    {
        if (!isActivated)
        {
            isActivated = true; // Activate enemy
           //ChaseTarget();
        }
    }

    void FindingTarget()
    {
        agent.speed = slowSpeed; // Slow down enemy while avoiding
        animator.SetTrigger("Walk");
        Vector3 avoidDirection = FindNewPath();

        if (avoidDirection != Vector3.zero) // Ensure we found a valid point
        {
            agent.SetDestination(avoidDirection);
            Debug.Log("Avoiding light: Moving to " + avoidDirection);
        }
        else
        {
            Debug.LogWarning("No valid avoidance position found, staying in place!");
        }
    }
    
    void ChaseTarget()
    {
        agent.SetDestination(target.position);
        agent.speed = normalSpeed;
        animator.SetTrigger("Run");
    }

    Vector3 FindNewPath()
    {
        for (int i = 0; i < 10; i++) // Try multiple times to find a valid position
        {
            Vector3 randomDirection = Random.insideUnitSphere * avoidDistance;
            randomDirection += agent.transform.position;

            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit navHit, avoidDistance, NavMesh.AllAreas))
            {
                return navHit.position; // Found a valid position
            }
        }
        return Vector3.zero; // No valid position found
    }

    void Attack()
    {
        if (Vector3.Distance(transform.position, target.position) <= attackRange)
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);
                foreach (var hitCollider in hitColliders)
                {
                    if (hitCollider.transform == transform)
                    {
                        IsAttacking = true; 
                        transform.LookAt(hitCollider.transform.position);
                        animator.SetTrigger("Scream");
                        PickUpManager.instance.CurntBatteryLevel -= 5;
                        Debug.Log("Enemy is attacking the target!");
                        lastAttackTime = Time.time;
                        Destroy(gameObject,2);
                        break;
                    }
                    else
                    {
                        IsAttacking = false;
                    }
                   
                }
            }
        }
    }

    // Draw Attack Radius in Scene View
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}