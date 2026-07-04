using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EnemyVision))]
public class EnemyAI : MonoBehaviour
{
    private enum EnemyState
    {
        Patrol,
        Chase
    }

    [Header("References")]
    [SerializeField] private AStarPathfinder pathfinder;
    [SerializeField] private PatrolRoute patrolRoute;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;

    [Header("Patrol")]
    [SerializeField] private float waitTimeAtWaypoint = 1f;

    [Header("Pathfinding")]
    [SerializeField] private float repathInterval = 0.5f;
    [SerializeField] private float waypointReachDistance = 0.05f;

    private Rigidbody2D rb;
    private EnemyVision enemyVision;

    private EnemyState currentState = EnemyState.Patrol;

    private List<GridNode> currentPath = new List<GridNode>();
    private int currentWaypointIndex;

    private int patrolIndex;
    private float repathTimer;
    private float waitTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyVision = GetComponent<EnemyVision>();
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
            return;

        if (enemyVision.PlayerInSight)
        {
            currentState = EnemyState.Chase;
        }
        else
        {
            currentState = EnemyState.Patrol;
        }

        switch (currentState)
        {
            case EnemyState.Patrol:
                UpdatePatrol();
                break;

            case EnemyState.Chase:
                UpdateChase();
                break;
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
            return;

        FollowPath();
    }

    private void UpdateChase()
    {
        repathTimer += Time.deltaTime;

        if (repathTimer >= repathInterval)
        {
            repathTimer = 0f;

            if (enemyVision.Player != null)
            {
                currentPath = pathfinder.FindPath(
                    transform.position,
                    enemyVision.Player.position);

                currentWaypointIndex = 0;
            }
        }
    }

    private void UpdatePatrol()
    {
        if (patrolRoute == null || patrolRoute.WaypointCount == 0)
            return;

        if (currentPath.Count == 0)
        {
            CalculatePatrolPath();
            return;
        }

        if (currentWaypointIndex >= currentPath.Count)
        {
            waitTimer += Time.deltaTime;

            if (waitTimer >= waitTimeAtWaypoint)
            {
                waitTimer = 0f;

                patrolIndex++;

                if (patrolIndex >= patrolRoute.WaypointCount)
                    patrolIndex = 0;

                CalculatePatrolPath();
            }
        }
    }

    private void CalculatePatrolPath()
    {
        Transform target = patrolRoute.GetWaypoint(patrolIndex);

        if (target == null)
            return;

        currentPath = pathfinder.FindPath(
            transform.position,
            target.position);

        currentWaypointIndex = 0;
    }

    private void FollowPath()
    {
        if (currentPath == null || currentPath.Count == 0)
            return;

        while (currentWaypointIndex < currentPath.Count)
        {
            float distance = Vector2.Distance(
                rb.position,
                currentPath[currentWaypointIndex].worldPosition);

            if (distance > waypointReachDistance)
                break;

            currentWaypointIndex++;
        }

        if (currentWaypointIndex >= currentPath.Count)
            return;

        Vector2 targetPosition =
            currentPath[currentWaypointIndex].worldPosition;

        Vector2 direction =
            (targetPosition - rb.position).normalized;

        rb.MovePosition(
            rb.position +
            direction * moveSpeed * Time.fixedDeltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (GameManager.Instance == null)
            return;

        if (GameManager.Instance.IsGameOver)
            return;

        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.EndGame();
        }
    }

    private void OnDrawGizmos()
    {
        if (currentPath == null)
            return;

        Gizmos.color = Color.cyan;

        foreach (GridNode node in currentPath)
        {
            Gizmos.DrawSphere(node.worldPosition, 0.08f);
        }

        for (int i = 0; i < currentPath.Count - 1; i++)
        {
            Gizmos.DrawLine(
                currentPath[i].worldPosition,
                currentPath[i + 1].worldPosition);
        }
    }
}