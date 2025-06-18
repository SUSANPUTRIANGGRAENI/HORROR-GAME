using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;

    // Patrol
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange = 10f;

    // State
    public float sightRange = 10f, attackRange = 2f;
    public bool playerInSightRange, playerInAttackRange;

    private bool isDead = false;

    // Animator
    private Animator animator;

    private void Awake()
    {
        GameObject playerObj = GameObject.Find("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError("Player 'Player' not found!");
        }

        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (agent == null) Debug.LogError("NavMeshAgent not found on enemy!");
        if (animator == null) Debug.LogError("Animator not found on enemy!");

        agent.updateRotation = false;
    }

    private void Update()
    {
        if (isDead || player == null) return;

        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (animator != null)
        {
            bool isWalking = agent.remainingDistance > agent.stoppingDistance && agent.velocity.magnitude > 0.1f;
            animator.SetBool("IsWalking", isWalking);
        }

        if (!playerInSightRange && !playerInAttackRange)
            Patroling();
        else if (playerInSightRange && !playerInAttackRange)
            ChasePlayer();
        else if (playerInAttackRange)
            ChasePlayer();
    }

    private void Patroling()
    {
        if (!walkPointSet)
            SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        Vector3 potentialPoint = new Vector3(transform.position.x + randomX, transform.position.y + 10f, transform.position.z + randomZ);

        if (Physics.Raycast(potentialPoint, Vector3.down, out RaycastHit hit, 20f, whatIsGround))
        {
            walkPoint = hit.point;
            walkPointSet = true;
        }
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
        FaceTarget();
    }

    private void FaceTarget()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0f;

        if (direction.magnitude > 0.1f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
