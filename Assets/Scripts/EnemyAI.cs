using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.GlobalIllumination;

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
    public bool IsSpoted = false;
    public bool IsTargetFound;



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
        if (!isActivated)
            return;

        switch (type)
        {
            case EnemytYPE.LightChaser:
                if (IsSpoted)
                {
                    if (!IsAttacking)
                        ChaseTarget();

                    IsTargetFound = false;
                }
                else
                {
                    if (!IsAttacking)
                        FindingTarget();
                }
                break;

            case EnemytYPE.LightAvoider:
                if (IsSpoted)
                {
                    if (!IsAttacking)
                        FindingTarget();
                }
                else
                {
                    if (!IsAttacking)
                        ChaseTarget();

                    IsTargetFound = false;
                }
                break;
        }

        Attack();
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
        if (IsTargetFound && agent.remainingDistance > agent.stoppingDistance)
            return;

        agent.speed = slowSpeed; // Slow down enemy while avoiding
        animator.SetTrigger("Walk");
        IsTargetFound = false;

        if (TryFindNewPath(out Vector3 point)) // Ensure we found a valid point
        {
            IsTargetFound = true;
            agent.SetDestination(point);
            Debug.Log("Avoiding light: Moving to " + point);
        }
        else
        {
            Debug.LogWarning("No valid avoidance position found, staying in place!");
        }
    }

    public void ChaseTarget()
    {
        agent.SetDestination(target.position);
        agent.speed = normalSpeed;
        animator.SetTrigger("Run");
    }

    bool TryFindNewPath(out Vector3 pointOnNavMesh)
    {
        static Vector3 AbsY(Vector3 p) => new(p.x, Mathf.Abs(p.y), p.z);

        for (int i = 0; i < 10; i++) // Try multiple times to find a valid position
        {
            Vector3 randomPointInsideHemisphere = AbsY(Random.insideUnitSphere);
            Vector3 randomPosition = agent.transform.position + randomPointInsideHemisphere * avoidDistance;

            if (NavMesh.SamplePosition(randomPosition, out NavMeshHit navHit, avoidDistance, NavMesh.AllAreas))
            {
                pointOnNavMesh = navHit.position; // Found a valid position
                return true;
            }
        }

        pointOnNavMesh = Vector3.positiveInfinity;
        return false; // No valid position found
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
                        Outline  Outlinerenderer = GetComponent<Outline>();
                        Outlinerenderer.enabled = true;
                        transform.LookAt(hitCollider.transform.position);
                        animator.SetTrigger("Scream");
                        SoundManager.instance.PlayScream();
                        //StartCoroutine(FlickerLight());
                        PickUpManager.instance.CurntBatteryLevel -= 5;
                        Debug.Log("Enemy is attacking the target!");
                        lastAttackTime = Time.time;
                        Destroy(gameObject,2);
                        break;
                    }
                    else
                    {
                        IsAttacking = false;
                        Outline Outlinerenderer = GetComponent<Outline>();
                        Outlinerenderer.enabled = false;
                    }
                   
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {    // Draw Attack Radius in Scene View

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, avoidDistance);
    }

    public IEnumerator FlickerLight()
    {
        if (IsAttacking)
        {
            FlashLight light = GameObject.FindObjectOfType<FlashLight>();
            light.spotlight = null;
            light.spotlight.enabled = !light.spotlight.enabled;
            yield return new WaitForSeconds(Random.Range(0.1f, 0.2f));
        }
        yield return null;
    }
}