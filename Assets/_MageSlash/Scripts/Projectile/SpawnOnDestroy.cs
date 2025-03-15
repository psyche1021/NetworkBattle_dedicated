using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnOnDestroy : MonoBehaviour
{
    [SerializeField] GameObject dustPrefab;

    void OnDestroy()
    {
        Instantiate(dustPrefab).transform.position = transform.position;
    }

}
