using UnityEngine;
using UnityEngine.AI;

public class MonsterManager : MonoBehaviour
{
    [Header("Scene References")]
    public Transform playerTransform;
    public GameObject playerTorch;

    [Header("Dynamic Search Settings")]
    public string monsterTag = "Monster";
    public string fogChildObjectName = "Volumetric Fog Volume";

    private GameObject monsterObject;
    private NavMeshAgent monsterAgent;
    private BoxCollider fogCollider;

    [Header("Patrol Settings")]
    public float patrolRadius = 15f;
    public float patrolSpeed = 2f;
    public float waitTimeAtPoint = 2f;

    [Header("Action Settings")]
    public float chaseSpeed = 5.5f;
    public float fleeSpeed = 6.5f;
    public float fleeDistance = 8f;
    public float catchDistance = 1.5f;

    private Vector3 initialPosition;
    private float waitTimer;
    private bool isChasingOrFleeing = false;

    private void Start()
    {
        InitializeMonsterData();
    }

    private void InitializeMonsterData()
    {
        monsterObject = GameObject.FindWithTag(monsterTag);

        if (monsterObject != null)
        {
            monsterAgent = monsterObject.GetComponent<NavMeshAgent>();

            if (monsterAgent != null)
            {
                initialPosition = monsterAgent.transform.position;
                monsterAgent.speed = patrolSpeed;
                SetNewPatrolDestination();
            }

            Transform fogTransform = monsterObject.transform.Find(fogChildObjectName);
            if (fogTransform != null)
            {
                fogCollider = fogTransform.GetComponent<BoxCollider>();
            }
            else
            {
                BoxCollider[] allBoxColliders = monsterObject.GetComponentsInChildren<BoxCollider>();
                foreach (BoxCollider bc in allBoxColliders)
                {
                    if (bc.gameObject.name == fogChildObjectName)
                    {
                        fogCollider = bc;
                        break;
                    }
                }
            }

            if (fogCollider == null)
            {
                Debug.LogWarning("MonsterManager: Fog BoxCollider not found on monster's children.");
            }
        }
        else
        {
            Debug.LogError("MonsterManager: Cannot find Monster in the scene by tag.");
        }
    }

     

    private void Update()
    {
        if (monsterAgent == null || playerTransform == null) return;
        if (!monsterAgent.isActiveAndEnabled || !monsterAgent.isOnNavMesh) return;

        float distanceToPlayer = Vector3.Distance(monsterAgent.transform.position, playerTransform.position);

        bool isTorchOn = playerTorch != null && playerTorch.activeInHierarchy;

        bool isPlayerInFog = false;
        if (fogCollider != null)
        {
            isPlayerInFog = fogCollider.bounds.Contains(playerTransform.position);
        }

       
         

        if (distanceToPlayer <= catchDistance && !isTorchOn)
        {
            StopAndCatchPlayer();
        }
        else if (isPlayerInFog && isTorchOn)
        {
            AvoidPlayer();
        }
        else if (isPlayerInFog && !isTorchOn)
        {
            ChasePlayer();
        }
        else
        {
            PatrolArea();
        }
    }

    private void StopAndCatchPlayer()
    {
        monsterAgent.isStopped = true;
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
        monsterAgent.isStopped = false;
        isChasingOrFleeing = true;
        monsterAgent.speed = chaseSpeed;
        monsterAgent.SetDestination(playerTransform.position);
    }

    private void AvoidPlayer()
    {
        monsterAgent.isStopped = false;
        isChasingOrFleeing = true;
        monsterAgent.speed = fleeSpeed;

        Vector3 directionAway = (monsterAgent.transform.position - playerTransform.position).normalized;
        Vector3 avoidTargetPosition = monsterAgent.transform.position + directionAway * fleeDistance;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(avoidTargetPosition, out hit, fleeDistance, NavMesh.AllAreas))
        {
            monsterAgent.SetDestination(hit.position);
        }
    }

    private void PatrolArea()
    {
        monsterAgent.isStopped = false;

        if (isChasingOrFleeing)
        {
            isChasingOrFleeing = false;
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
}