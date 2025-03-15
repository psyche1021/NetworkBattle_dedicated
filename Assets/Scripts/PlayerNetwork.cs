using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{

    [SerializeField] GameObject spawnedObjectPrefab;
    GameObject spawnedObject;

    struct SomeData : INetworkSerializable
    {
        public bool _bool;
        public int _int;
        public FixedString32Bytes _string;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _bool);
            serializer.SerializeValue(ref _int);
            serializer.SerializeValue(ref _string);
        }
    }

    NetworkVariable<SomeData> randomNumber = new NetworkVariable<SomeData>( new SomeData
    {
        _bool = true,
        _int = 0
    },
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        randomNumber.OnValueChanged += HandleRandomNumber;
    }

    public override void OnNetworkDespawn()
    {
        randomNumber.OnValueChanged -= HandleRandomNumber;
    }

    void HandleRandomNumber(SomeData oldValue, SomeData newValue)
    {
        Debug.Log("owner id : " + OwnerClientId + " random number :" + randomNumber.Value._int + " " + randomNumber.Value._string);
    }

    // Update is called once per frame
    void Update()
    {

        if (!IsOwner) { return; }

//        Debug.Log("is server " + IsServer + " is client " + IsClient);

        if (Input.GetKeyDown(KeyCode.X))
        {
            spawnedObject = Instantiate(spawnedObjectPrefab);
            spawnedObject.GetComponent<NetworkObject>().Spawn(true);   // spawn의 권한은 서버에 있다
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            spawnedObject.GetComponent<NetworkObject>().Despawn();  // 두둥 게임 오브젝트가 파괴된다
            Debug.Log(spawnedObject);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
//            TestServerRpc(Random.Range(100, 1000));
//            TestServerRpc(" why???");
//            TestServerRpc(new ServerRpcParams());
//            TestClientRpc();
            TestClientRpc(new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new List<ulong>{ 1 }
                }
            });
        }

        if (Input.GetKeyDown(KeyCode.T) )
        {
            randomNumber.Value = new SomeData { 
                _int = Random.Range(0, 100) , 
                _string= "일이삼사오륙칠팔구" 
            };
        }

        Vector3 moveDir = Vector3.zero;

        moveDir.x = Input.GetAxis("Horizontal");
        moveDir.z = Input.GetAxis("Vertical");

        float moveSpeed = 3;

        transform.Translate(moveDir * moveSpeed * Time.deltaTime);
    }

    [ServerRpc]
//    void TestServerRpc(string str)
    void TestServerRpc(ServerRpcParams rpcParams)
    {
        Debug.Log("Server RPC called "  + rpcParams.Receive.SenderClientId);
    }

    [ClientRpc]
    void TestClientRpc(ClientRpcParams rpcParams)
    {
        Debug.Log("Client RPC Called");
    }
}
