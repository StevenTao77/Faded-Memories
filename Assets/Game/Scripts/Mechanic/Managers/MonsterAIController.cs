using UnityEngine;
using UnityEngine.AI;

public class MonsterManager : MonoBehaviour
{
    [Header("Scene References")]
    public Transform playerTransform;
    private NavMeshAgent monsterAgent;

    [Header("Patrol Settings")]
    public float patrolRadius = 15f;
    public float patrolSpeed = 2f;
    public float waitTimeAtPoint = 2f;

    [Header("Detection Settings")]
    public float detectionRadius = 12f;
    [Range(0, 360)] public float viewAngle = 110f;

    [Header("Chase & Catch Settings")]
    public float chaseSpeed = 5.5f;

    //The distance at which the monster "collides" or catches the player
    public float catchDistance = 1.5f;

    private Vector3 initialPosition;
    private float waitTimer;
    private bool isChasing = false;

    private void Start()
    {
        FindMonsterInEnvironment();
    }

    private void FindMonsterInEnvironment()
    {
        GameObject enemyObj = GameObject.FindWithTag("Monster");
        if (enemyObj != null)
        {
            monsterAgent = enemyObj.GetComponent<NavMeshAgent>();
            if (monsterAgent != null)
            {
                initialPosition = monsterAgent.transform.position;
                monsterAgent.speed = patrolSpeed;
                SetNewPatrolDestination();
            }
        }
        else
        {
            Debug.LogWarning("MonsterManager: No GameObject with tag 'Enemy' found.");
        }
    }

    private void Update()
    {
        if (monsterAgent == null || playerTransform == null) return;
        if (!monsterAgent.isActiveAndEnabled || !monsterAgent.isOnNavMesh) return;

        float distanceToPlayer = Vector3.Distance(monsterAgent.transform.position, playerTransform.position);

        // Core Logic Update: 3-Tier State Machine
      
        if (distanceToPlayer <= catchDistance)
        {
            StopAndCatchPlayer();
        }
    
        else if (CanSeePlayer(distanceToPlayer))
        {
            ChasePlayer();
        }
       
        else
        {
            PatrolArea();
        }
    }

    private bool CanSeePlayer(float distanceToPlayer)
    {
        if (distanceToPlayer > detectionRadius) return false;

        Vector3 directionToPlayer = (playerTransform.position - monsterAgent.transform.position).normalized;
        float angleBetweenEnemyAndPlayer = Vector3.Angle(monsterAgent.transform.forward, directionToPlayer);

        if (angleBetweenEnemyAndPlayer < viewAngle / 2f)
        {
            return true;
        }

        return false;
    }

    private void StopAndCatchPlayer()
    {
        // Force the agent to stop moving
        monsterAgent.isStopped = true;

        // Keep looking at the player while stopped
        Vector3 direction = (playerTransform.position - monsterAgent.transform.position).normalized;
        direction.y = 0;  

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            monsterAgent.transform.rotation = Quaternion.Slerp(monsterAgent.transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    private void ChasePlayer()
    {
        // Allow the agent to move again
        monsterAgent.isStopped = false;

        isChasing = true;
        monsterAgent.speed = chaseSpeed;
        monsterAgent.SetDestination(playerTransform.position);
    }

    private void PatrolArea()
    {
        monsterAgent.isStopped = false;

        if (isChasing)
        {
            isChasing = false;
            monsterAgent.speed = patrolSpeed;
            SetNewPatrolDestination();
        }

        if (!monsterAgent.pathPending && monsterAgent.remainingDistance <= monsterAgent.stoppingDistance)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTimeAtPoint)
            {
                SetNewPatrolDestination();
                waitTimer = 0f;
            }
        }
    }

    private void SetNewPatrolDestination()
    {
        if (monsterAgent == null) return;

        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += initialPosition;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, 1))
        {
            monsterAgent.SetDestination(hit.position);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (monsterAgent == null) return;
         
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(monsterAgent.transform.position, detectionRadius);
         
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(monsterAgent.transform.position, catchDistance);
         
        Vector3 leftBoundary = DirFromAngle(-viewAngle / 2f, false);
        Vector3 rightBoundary = DirFromAngle(viewAngle / 2f, false);
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(monsterAgent.transform.position, monsterAgent.transform.position + leftBoundary * detectionRadius);
        Gizmos.DrawLine(monsterAgent.transform.position, monsterAgent.transform.position + rightBoundary * detectionRadius);
         
        if (playerTransform != null)
        {
            float distance = Vector3.Distance(monsterAgent.transform.position, playerTransform.position);

            if (distance <= catchDistance)
            { 
                Gizmos.color = Color.green;
                Gizmos.DrawLine(monsterAgent.transform.position, playerTransform.position);
            }
            else if (CanSeePlayer(distance))
            { 
                Gizmos.color = Color.red;
                Gizmos.DrawLine(monsterAgent.transform.position, playerTransform.position);
            }
        }
    }

    private Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += monsterAgent.transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}