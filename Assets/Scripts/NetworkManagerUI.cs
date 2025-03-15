using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkManagerUI : MonoBehaviour
{
    public void HostPressed()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void ClientPressed()
    {
        NetworkManager.Singleton.StartClient();
    }

}
