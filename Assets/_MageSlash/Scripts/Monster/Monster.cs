using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Monster : NetworkBehaviour
{

    public override void OnNetworkSpawn()
    {
        GetComponent<Health>().OnDie += OnDie; 
    }

    public override void OnNetworkDespawn()
    {
        GetComponent<Health>().OnDie -= OnDie;
    }

    void OnDie(Health sender)
    {
        // 죽을때 뭔가 애니메이션이나 특수효과를 넣고 싶으면 여기서 넣고 시간 지난 다음에 디스트로이 해도 됩니다
        gameObject.GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject);

    }
}
