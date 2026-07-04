using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    [Header("Vision Settings")]
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private LayerMask playerLayer;

    public bool PlayerInSight { get; private set; }
    public Transform Player { get; private set; }

    private void Update()
    {
        DetectPlayer();
    }

    private void DetectPlayer()
    {
        Collider2D hit = Physics2D.OverlapCircle(
            transform.position,
            detectionRadius,
            playerLayer);

        if (hit != null)
        {
            PlayerInSight = true;
            Player = hit.transform;
        }
        else
        {
            PlayerInSight = false;
            Player = null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = PlayerInSight ? Color.red : Color.green;

        Gizmos.DrawWireSphere(
            transform.position,
            detectionRadius);
    }
}