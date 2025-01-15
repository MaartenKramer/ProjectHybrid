using UnityEngine;
using UnityEngine.AI;

public class EnemyStateMachine : MonoBehaviour
{
    public AudioClip suspiciousSound;
    public AudioClip alertSound;
    private AudioSource audioSource;

    private bool hasPlayedSuspiciousSound = false;
    private bool hasPlayedAlertSound = false;

    private Animator animator;

    public Transform[] waypoints;
    public Transform visibilityCone;
    public float spottingTime = 2f;
    public float chaseStoppingDistance = 2f;
    public float gracePeriod = 2f;

    private int currentWaypoint = 0;
    private NavMeshAgent agent;
    private Transform player;
    private float spottingTimer = 0f;
    private float resetTimer = 0f;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private int initialWaypoint;

    private enum EnemyState { Patrol, Alert, Chase, WaitBeforeReset, Reset }
    private EnemyState currentState = EnemyState.Patrol;

    private void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (player == null)
        {
            Debug.LogError("Player not found. Ensure the Player GameObject has the tag 'Player'.");
            return;
        }

        initialPosition = transform.position;
        initialRotation = transform.rotation;
        initialWaypoint = currentWaypoint;

        if (waypoints.Length > 0)
        {
            agent.SetDestination(waypoints[currentWaypoint].position);
        }
    }

    private void Update()
    {
        switch (currentState)
        {
            case EnemyState.Patrol: Patrol(); break;
            case EnemyState.Alert: Alert(); break;
            case EnemyState.Chase: Chase(); break;
            case EnemyState.WaitBeforeReset: WaitBeforeReset(); break;
            case EnemyState.Reset: ResetState(); break;
        }
    }

    private void Patrol()
    {
        if (agent.remainingDistance < 0.5f && !agent.pathPending)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
            agent.SetDestination(waypoints[currentWaypoint].position);
            animator.SetBool("Idle", false);
            animator.SetBool("Walking", true);
        }

        if (PlayerInCone())
        {
            currentState = EnemyState.Alert;
        }
        hasPlayedSuspiciousSound = false;
    }

    private void Alert()
    {
        animator.SetBool("Idle", false);
        animator.SetBool("Walking", true);
        agent.isStopped = true;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 60f * Time.deltaTime);

        spottingTimer += Time.deltaTime;
        UpdateConeColor(spottingTimer / spottingTime);

        if (spottingTimer >= spottingTime)
        {
            agent.isStopped = false;
            currentState = EnemyState.Chase;
        }
        else if (!PlayerInCone())
        {
            spottingTimer -= Time.deltaTime * 2f;
            if (spottingTimer <= 0)
            {
                spottingTimer = 0;
                agent.isStopped = false;
                currentState = EnemyState.Patrol;
            }
        }
    }

    private void Chase()
    {
        animator.SetBool("Idle", false);
        animator.SetBool("Walking", true);
        if (!hasPlayedAlertSound)
        {
            PlaySound(alertSound);
            hasPlayedAlertSound = true;
        }

        agent.SetDestination(player.position);
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= chaseStoppingDistance)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            agent.ResetPath();
            transform.position = Vector3.MoveTowards(transform.position, player.position, -0.01f);
            resetTimer = gracePeriod;
            currentState = EnemyState.WaitBeforeReset;
        }
    }

    private void WaitBeforeReset()
    {
        animator.SetBool("Idle", true);
        animator.SetBool("Walking", false);

        if (resetTimer == gracePeriod)
        {
            FadeManager.Instance.FadeOut();
        }

        resetTimer -= Time.deltaTime;

        if (resetTimer <= 0)
        {
            CheckpointManager.Instance.ResetToCheckpoint(player.gameObject);
            currentState = EnemyState.Reset;
        }
    }

    private void ResetState()
    {
        ResetToInitialState();
    }

    public void ResetToInitialState()
    {
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent is missing or was not initialized for " + gameObject.name);
            return;
        }

        // Reset position, rotation, and path
        transform.position = initialPosition;
        transform.rotation = initialRotation;

        currentWaypoint = initialWaypoint;
        agent.isStopped = false;
        agent.ResetPath();

        if (waypoints.Length > 0)
        {
            agent.SetDestination(waypoints[currentWaypoint].position);
        }

        spottingTimer = 0;
        resetTimer = 0;
        hasPlayedSuspiciousSound = false;
        hasPlayedAlertSound = false;

        UpdateConeColor(0);
        currentState = EnemyState.Patrol;
        Debug.Log($"Enemy {gameObject.name} reset to initial state.");
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
                    if (!hasPlayedSuspiciousSound)
                    {
                        PlaySound(suspiciousSound);
                        hasPlayedSuspiciousSound = true;
                    }
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
            Color baseColor = Color.yellow;

            Color.RGBToHSV(baseColor, out float hue, out float saturation, out float value);
            hue = Mathf.Lerp(hue, 0f, t);

            Color shiftedColor = Color.HSVToRGB(hue, saturation, value);
            shiftedColor.a = coneRenderer.material.color.a;

            coneRenderer.material.color = shiftedColor;
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
