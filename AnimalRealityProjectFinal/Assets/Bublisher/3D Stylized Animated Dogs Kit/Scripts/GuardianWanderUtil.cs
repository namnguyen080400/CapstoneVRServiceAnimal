using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public static class GuardianWanderUtil
{
    public static Vector3 FindReachableDestinationInGuardianArc(
        Transform player,
        Transform navMeshAgent,
        int arcSamples = 18,
        float minStep = 0.5f,
        float maxStep = 4.0f,
        float stepInterval = 0.1f,
        float navMeshSampleRadius = 0.5f)
    {
        // Generate arc directions in front of player
        List<Vector3> directions = new List<Vector3>();
        for (int i = 0; i < arcSamples; i++)
        {
            float angle = Mathf.Lerp(-90f, 90f, i / (arcSamples - 1f));
            Vector3 dir = Quaternion.Euler(0, angle, 0) * Vector3.forward;
            Quaternion yawOnly = Quaternion.Euler(0, player.eulerAngles.y, 0);
            Vector3 worldDir = yawOnly * dir.normalized;
            directions.Add(worldDir);
        }

        directions.Shuffle();

/*         foreach (var d in directions)
            Debug.Log($"Nam11 direction: {d}"); */

        foreach (Vector3 dir in directions)
        {
            for (float d = maxStep; d >= minStep; d -= stepInterval)
            {
                Vector3 point = player.position + dir * d;
                Vector3 flatPoint = new Vector3(point.x, 0f, point.z);

                NavMeshPath path = new NavMeshPath();
                if (NavMesh.CalculatePath(navMeshAgent.position, flatPoint, NavMesh.AllAreas, path) &&
                    path.status == NavMeshPathStatus.PathComplete)
                {
                    Debug.Log($"Nam11 Found valid point: {flatPoint} camera position = {player.position} navMeshAgent.position = {navMeshAgent.position}");
                    return flatPoint;
                }
                else
                {
                    //Debug.Log($"Nam11 Not a valid point path.status = {path.status} flatPoint = {flatPoint}");
                }
            }
        }

        Debug.LogWarning("Nam11 No reachable point found within guardian arc.");
        return Vector3.zero;
    }

    private static bool IsPointInPolygonXZ(Vector3 point, List<Vector3> polygon)
    {
        int crossings = 0;
        for (int i = 0; i < polygon.Count; i++)
        {
            Vector3 a = new Vector3(polygon[i].x, 0f, polygon[i].z);
            Vector3 b = new Vector3(polygon[(i + 1) % polygon.Count].x, 0f, polygon[(i + 1) % polygon.Count].z);

            if ((a.z > point.z) != (b.z > point.z))
            {
                float t = (point.z - a.z) / (b.z - a.z);
                float x = a.x + t * (b.x - a.x);
                if (x > point.x) crossings++;
            }
        }
        return (crossings % 2) == 1;
    }

    public static void Shuffle<T>(this IList<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int j = Random.Range(i, list.Count);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
} 
