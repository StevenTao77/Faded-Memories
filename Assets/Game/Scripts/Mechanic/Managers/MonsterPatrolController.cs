using UnityEngine;
using UnityEngine.AI;

public class MonsterPatrolController : MonoBehaviour
{
    [Header("Target Settings")] 
    public string monsterTag = "Monster";

    [Header("Patrol Settings")]
    public float patrolRadius = 20f;
    public float waitTime = 3f;

    private NavMeshAgent agent;
    private float waitTimer;

    void Start()
    {
         
        GameObject monsterObject = GameObject.FindGameObjectWithTag(monsterTag);

        if (monsterObject != null)
        {
             
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
         
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            waitTimer += Time.deltaTime;
             
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
         
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }
}