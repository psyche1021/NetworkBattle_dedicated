using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    static List<Vector3> spawnPoints = new List<Vector3>();

    void OnEnable()
    {
        spawnPoints.Add(transform.position);
    }

    void OnDisable()
    {
        spawnPoints.Remove(transform.position);
    }

    public static Vector3 GetRandomSpawnPoint()
    {
        if (spawnPoints.Count == 0)
        {
            return Vector3.zero;
        }
        return spawnPoints[Random.Range(0, spawnPoints.Count)];
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, 1);
    }

}
