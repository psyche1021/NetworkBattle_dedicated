using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MagicPoint : NetworkBehaviour
{
    public float maxMagicPoint = 100;
    public NetworkVariable<float> currentMagic = 
        new NetworkVariable<float>(0,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);
    [SerializeField] float regenerateRate = 2f;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { return; }

        currentMagic.Value = maxMagicPoint;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

        RestoreMagic(regenerateRate* Time.deltaTime);
    }

    public bool UseMagic(float magic)
    {
        if (currentMagic.Value < magic) return false;
        ModifyMagicPoint(-magic);
        return true;
    }

    [ClientRpc]
    public void RestoreMagicClientRpc(float magic, ClientRpcParams clientRpcParams)
    {
        RestoreMagic(magic);
    }

    public void RestoreMagic(float magic)
    {
        ModifyMagicPoint(magic);
    }

    void ModifyMagicPoint(float value)
    {
        currentMagic.Value = Mathf.Clamp(currentMagic.Value + value, 0, maxMagicPoint);
    }
}
