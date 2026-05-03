using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class MonsterManager : MonoBehaviour
{
    [Header("Scene References")]
    public Transform playerTransform;
    public GameObject playerTorch;

    [Header("Dynamic Search Settings")]
    public string monsterTag = "Monster";
    public string fogChildObjectName = "Volumetric Fog Volume";
    public string torchObjectName = "torch";

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
    private float fleeRecalculateTimer = 0f;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeMonsterData();

        if (playerTransform == null || playerTorch == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;

                Transform torchTransform = FindChildRecursively(player.transform, torchObjectName);

                if (torchTransform != null)
                {
                    playerTorch = torchTransform.gameObject;
                }
                else
                {
                    Debug.LogWarning($"MonsterManager: Player found, but child named '{torchObjectName}' is missing. Torch detection will fail.");
                }
            }
        }
    }

    private Transform FindChildRecursively(Transform parent, string nameToFind)
    {
        foreach (Transform child in parent)
        {
            if (child.name == nameToFind)
                return child;

            Transform result = FindChildRecursively(child, nameToFind);
            if (result != null)
                return result;
        }
        return null;
    }

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

        fleeRecalculateTimer -= Time.deltaTime;

        // Optimize performance: Only recalculate escape route every 0.5 seconds
        if (fleeRecalculateTimer > 0f && monsterAgent.hasPath) return;

        fleeRecalculateTimer = 0.5f;

        Vector3 bestEscapePoint = monsterAgent.transform.position;
        float maxDistanceToPlayer = 0f;
        bool foundValidEscape = false;

        // Sample 8 directions in a circle around the monster
        for (int i = 0; i < 8; i++)
        {
            float angle = i * 45f;
            // Get direction away from player, then rotate it by current angle
            Vector3 baseDirAway = (monsterAgent.transform.position - playerTransform.position).normalized;
            Vector3 checkDirection = Quaternion.Euler(0, angle, 0) * baseDirAway;

            Vector3 potentialPosition = monsterAgent.transform.position + checkDirection * fleeDistance;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(potentialPosition, out hit, fleeDistance * 0.5f, NavMesh.AllAreas))
            {
                float distToPlayerFromHit = Vector3.Distance(hit.position, playerTransform.position);
                float currentDistToPlayer = Vector3.Distance(monsterAgent.transform.position, playerTransform.position);

                // Make sure this new point is actually further away than where we stand now
                if (distToPlayerFromHit > currentDistToPlayer)
                {
                    // CRITICAL FIX: Simulate the path to make sure it's not blocked by a wall
                    NavMeshPath path = new NavMeshPath();
                    if (monsterAgent.CalculatePath(hit.position, path))
                    {
                        // PathComplete means we can physically walk there without getting stuck
                        if (path.status == NavMeshPathStatus.PathComplete)
                        {
                            // Keep the point that gets us the FURTHEST away from the player
                            if (distToPlayerFromHit > maxDistanceToPlayer)
                            {
                                maxDistanceToPlayer = distToPlayerFromHit;
                                bestEscapePoint = hit.position;
                                foundValidEscape = true;
                            }
                        }
                    }
                }
            }
        }
         
        if (foundValidEscape)
        {
            monsterAgent.SetDestination(bestEscapePoint);
        }
        else
        {
            // Extreme Fallback: If completely cornered by walls AND player, try to slip to the side
            Vector3 slipPastPosition = monsterAgent.transform.position + (monsterAgent.transform.right * fleeDistance * 0.5f);
            NavMeshHit fallbackHit;
            if (NavMesh.SamplePosition(slipPastPosition, out fallbackHit, fleeDistance * 0.5f, NavMesh.AllAreas))
            {
                monsterAgent.SetDestination(fallbackHit.position);
            }
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