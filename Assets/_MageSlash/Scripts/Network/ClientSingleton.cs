using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientSingleton : MonoBehaviour
{
    static ClientSingleton instance;
    MatchplayMatchmaker matchmaker;
    UserData userData;

    public static ClientSingleton Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject singletonObject = new GameObject("ClientSingleton");
                instance = singletonObject.AddComponent<ClientSingleton>();

                DontDestroyOnLoad(singletonObject);
            }
            return instance;
        }
    }

    JoinAllocation allocation;

    public async Task<bool> InitAsync()
    {
        await UnityServices.InitializeAsync();

        AuthState state = await Authenticator.DoAuth();

        matchmaker = new MatchplayMatchmaker();

        if (state == AuthState.Authenticated)
        {
            userData = new UserData()
            {
                userName = AuthenticationService.Instance.PlayerName ?? "Unknown",
                userAuthId = AuthenticationService.Instance.PlayerId
            };

            NetworkManager.Singleton.OnClientDisconnectCallback += OnDisconnected;
            return true;
        }

        return false;
    }

    private void OnDisconnected(ulong clientId)
    {
        if (clientId != 0 && clientId != NetworkManager.Singleton.LocalClientId)
        {
            // ������ �������ϴ� ó���� �ʿ��ϴٸ� ����
            // ������ authid�� ������ �ִٰ� �ٽ� ������ ��� �÷��� �ϵ���
            return;
        }

        if (SceneManager.GetActiveScene().name != "MenuScene")
        {
            SceneManager.LoadScene("MenuScene");
        }
    }

    public async Task StartClientAsync(string joinCode)
    {

        try
        {
            allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
            return;
        }

        UnityTransport transport= NetworkManager.Singleton.GetComponent<UnityTransport>();
        RelayServerData relayServerData = allocation.ToRelayServerData("dtls");
        transport.SetRelayServerData(relayServerData);

        ConnectClient();
    }

    public async void MatchmakeAsync(Action<MatchmakerPollingResult> onMatchmakeResponse)
    {
        if (matchmaker.IsMatchmaking)
        {
            return;
        }

        MatchmakerPollingResult result = await GetMatchAsync();
        onMatchmakeResponse?.Invoke(result);
    }

    public async Task<MatchmakerPollingResult> GetMatchAsync()
    {
        MatchmakingResult result = await matchmaker.Matchmake(userData);

        Debug.Log(result.resultMessage);

        if (result.result == MatchmakerPollingResult.Success)
        {
            StartClient(result.ip,(ushort)result.port);
        }

        return result.result;
    }

    public void StartClient(string ip, ushort port)
    {
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetConnectionData(ip, port);

        ConnectClient();
    }

    void ConnectClient()
    {
        // payload �����

        string payload = JsonConvert.SerializeObject(userData);
        byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);

        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;

        NetworkManager.Singleton.StartClient();
    }

    public async Task CancelMatchmaking()
    {
        await matchmaker.CancelMatchmaking();
    }
}
