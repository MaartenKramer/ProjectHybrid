using UnityEngine;
using UnityEngine.AI;

public class EnemyStateMachine : MonoBehaviour
{
    public Transform[] waypoints;
    public Transform visibilityCone;
    public float spottingTime = 2f;
    public float chaseStoppingDistance = 2f;
    public float gracePeriod = 2f; // Time enemy waits before resetting

    private int currentWaypoint = 0;
    private NavMeshAgent agent;
    private Transform player;
    private float spottingTimer = 0f;
    private float resetTimer = 0f;
    private Vector3 initialPosition;

    private enum EnemyState { Patrol, Alert, Chase, WaitBeforeReset, Reset }
    private EnemyState currentState = EnemyState.Patrol;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        initialPosition = transform.position;
        agent.SetDestination(waypoints[currentWaypoint].position);
    }

    private void Update()
    {
        switch (currentState)
        {
            case EnemyState.Patrol:
                Patrol();
                break;
            case EnemyState.Alert:
                Alert();
                break;
            case EnemyState.Chase:
                Chase();
                break;
            case EnemyState.WaitBeforeReset:
                WaitBeforeReset();
                break;
            case EnemyState.Reset:
                ResetState();
                break;
        }
    }

    private void Patrol()
    {
        if (agent.remainingDistance < 0.5f && !agent.pathPending)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
            agent.SetDestination(waypoints[currentWaypoint].position);
        }

        if (PlayerInCone())
        {
            currentState = EnemyState.Alert;
        }
    }

    private void Alert()
    {
        spottingTimer += Time.deltaTime;
        UpdateConeColor(spottingTimer / spottingTime);

        if (spottingTimer >= spottingTime)
        {
            currentState = EnemyState.Chase;
            agent.stoppingDistance = chaseStoppingDistance;
        }
        else if (!PlayerInCone())
        {
            spottingTimer -= Time.deltaTime * 2f;
            if (spottingTimer <= 0)
            {
                spottingTimer = 0;
                currentState = EnemyState.Patrol;
            }
        }
    }

    private void Chase()
    {
        agent.SetDestination(player.position);

        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            currentState = EnemyState.WaitBeforeReset;
            resetTimer = gracePeriod;
        }
    }

    private void WaitBeforeReset()
    {
        resetTimer -= Time.deltaTime;

        if (resetTimer <= 0)
        {
            currentState = EnemyState.Reset;
        }
    }

    private void ResetState()
    {
        spottingTimer = 0;
        resetTimer = 0;
        UpdateConeColor(0);
        transform.position = initialPosition;
        currentWaypoint = 0;
        agent.SetDestination(waypoints[currentWaypoint].position);
        currentState = EnemyState.Patrol;
    }

    private bool PlayerInCone()
    {
        Collider coneCollider = visibilityCone.GetComponent<Collider>();
        if (coneCollider != null && coneCollider.bounds.Contains(player.position))
        {
            Vector3 directionToPlayer = player.position - visibilityCone.position;
            if (Physics.Raycast(visibilityCone.position, directionToPlayer.normalized, out RaycastHit hit))
            {
                if (hit.transform.CompareTag("Player"))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void UpdateConeColor(float t)
    {
        Renderer coneRenderer = visibilityCone.GetComponent<Renderer>();
        if (coneRenderer != null)
        {
            coneRenderer.material.color = Color.Lerp(Color.yellow, Color.red, t);
        }
    }
}
