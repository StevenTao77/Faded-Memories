using UnityEngine;
using UnityEngine.AI;

public class MonsterPatrolController : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("The tag of the monster in the environment scene.")]
    public string monsterTag = "Monster";

    [Header("Patrol Settings")]
    public float patrolRadius = 20f;
    public float waitTime = 3f;

    private NavMeshAgent agent;
    private float waitTimer;

    void Start()
    {
        // 1. Dynamically find the monster across scenes using its Tag
        GameObject monsterObject = GameObject.FindGameObjectWithTag(monsterTag);

        if (monsterObject != null)
        {
            // 2. Get the NavMeshAgent component from the environment entity
            agent = monsterObject.GetComponent<NavMeshAgent>();
        }
        else
        {
            Debug.LogError("Monster not found! Make sure the monster in the environment scene has the correct Tag.");
            return;
        }

        waitTimer = waitTime;
    }

    void Update()
    {
        if (agent == null) return;

        // 3. Check if the monster has reached its destination
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            waitTimer += Time.deltaTime;

            // 4. Wait for a few seconds, then find a new destination
            if (waitTimer >= waitTime)
            {
                SetNewRandomDestination();
                waitTimer = 0f;
            }
        }
    }

    void SetNewRandomDestination()
    {
        // Generate a random point within a sphere around the current position
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += agent.transform.position;

        NavMeshHit hit;

        // Sample the NavMesh to find the closest valid point on the baked mesh
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }
}