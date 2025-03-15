using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] TMP_Text findMatchStatusText;
    [SerializeField] TMP_Text findButtonText;

    [SerializeField] TMP_InputField joinCodeField;
    [SerializeField] TMP_InputField usernameField;

    bool isMatchmaking;
    bool isCancelling;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (GameObject.FindFirstObjectByType<NetworkManager>() == null)
        {
            SceneManager.LoadScene("NetConnectScene");
        }
        try {
            string username = AuthenticationService.Instance.PlayerName ?? "";
            if (username.Contains("#"))
            {
                username = username.Substring(0, username.LastIndexOf("#"));
            }
            usernameField.text = username;
        }
        catch
        {
        }
    }

    public async void StartHost()
    {
        await HostSingleton.Instance.StartHostAsync();
    }

    public async void StartClient()
    {
        await ClientSingleton.Instance.StartClientAsync(joinCodeField.text);
    }

    public async void ChangeName()
    {
        // ����� ������ ���� ��ĭ�� �Ұ����ؿ� ��� �޼����� 
        await AuthenticationService.Instance.UpdatePlayerNameAsync(usernameField.text);
    }

    public async void FindMatchPressed()
    {
        if (isCancelling) { return; }

        if (isMatchmaking)
        {
            // ��ҹ�ư
            isCancelling = true;
            findMatchStatusText.text = "Cancelling...";

            await ClientSingleton.Instance.CancelMatchmaking();

            isCancelling = false;
            isMatchmaking = false;
            findMatchStatusText.text = "";
            findButtonText.text = "Find Match";
        }
        else
        {
            // ��ġ

            findMatchStatusText.text = "Searching...";
            findButtonText.text = "Cancel";

            isMatchmaking = true;
            isCancelling = false;

            ClientSingleton.Instance.MatchmakeAsync(OnMatchMade);
        }
    }

    void OnMatchMade(MatchmakerPollingResult result)
    {
        switch (result)
        {
            case MatchmakerPollingResult.Success:
                findMatchStatusText.text = "connecting...";
                break;
            default:
                isMatchmaking = false;
                findMatchStatusText.text = "error " + result;
                break;
        }
    }

}
