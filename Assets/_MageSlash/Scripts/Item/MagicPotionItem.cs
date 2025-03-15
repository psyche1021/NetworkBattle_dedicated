using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MagicPotionItem : NetworkBehaviour
{
    [SerializeField] float magicPoint = 50;

    public float Collect()
    {
        if (!IsServer) return 0;

        Destroy(gameObject);

        return magicPoint;
    }
}
