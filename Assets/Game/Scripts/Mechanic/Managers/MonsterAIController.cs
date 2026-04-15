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

        // Re-find player and torch if references are lost after scene transition
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
            else
            {
                Debug.LogWarning("MonsterManager: Cannot find object with 'Player' tag after scene load.");
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

            if (fogCollider == null)
            {
                Debug.LogWarning("MonsterManager: Fog BoxCollider not found on monster's children.");
            }
        }
        else
        {
            Debug.LogWarning("MonsterManager: Cannot find Monster in the scene by tag. (This is normal if the current scene doesn't have a monster)");
        }
    }

    private void Update()
    {
        if (monsterAgent == null || playerTransform == null) return;
        if (!monsterAgent.isActiveAndEnabled || !monsterAgent.isOnNavMesh) return;

        float distanceToPlayer = Vector3.Distance(monsterAgent.transform.position, playerTransform.position);

        // Check if the torch is active in the hierarchy
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