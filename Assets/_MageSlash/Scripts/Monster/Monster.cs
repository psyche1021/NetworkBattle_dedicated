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
        // ������ ���� �ִϸ��̼��̳� Ư��ȿ���� �ְ� ������ ���⼭ �ְ� �ð� ���� ������ ��Ʈ���� �ص� �˴ϴ�
        gameObject.GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject);

    }
}
