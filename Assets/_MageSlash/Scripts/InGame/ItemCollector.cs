using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ItemCollector : NetworkBehaviour
{

    void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<MagicPotionItem>(out MagicPotionItem item)) return;

        float magic = item.Collect();

        if (!IsServer)
        {
            return;
        }

        GetComponent<MagicPoint>().RestoreMagicClientRpc(magic,
            new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new[] { OwnerClientId }
                }
            });
    }

}
