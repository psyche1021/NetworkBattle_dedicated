using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkSpawnOnDestoy : NetworkBehaviour
{
    [SerializeField] GameObject prefab;

    public override void OnDestroy()
    {
        base.OnDestroy();

        try
        {
            if (NetworkManager.Singleton.IsListening &&
                !NetworkManager.Singleton.ShutdownInProgress)
            {
                if (!IsServer) return;
                GameObject go = Instantiate(prefab);

                go.transform.position = transform.position;
                go.GetComponent<NetworkObject>().Spawn();
            }
        }
        catch { }
    }
}
