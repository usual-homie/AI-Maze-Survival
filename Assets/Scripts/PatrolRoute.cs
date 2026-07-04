using UnityEngine;

public class PatrolRoute : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;

    public int WaypointCount => waypoints.Length;

    public Transform GetWaypoint(int index)
    {
        if (waypoints == null || waypoints.Length == 0)
            return null;

        index %= waypoints.Length;

        return waypoints[index];
    }

    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length == 0)
            return;

        Gizmos.color = Color.yellow;

        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == null)
                continue;

            Gizmos.DrawSphere(waypoints[i].position, 0.12f);

            Transform next = waypoints[(i + 1) % waypoints.Length];

            if (next != null)
            {
                Gizmos.DrawLine(
                    waypoints[i].position,
                    next.position);
            }
        }
    }
}