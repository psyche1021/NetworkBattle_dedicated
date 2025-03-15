using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyItem : MonoBehaviour
{
    [SerializeField] TMP_Text lobbyTitleTmp;
    [SerializeField] TMP_Text lobbyPlayersTmp;

    LobbyList lobbyList;
    Lobby lobby;

    public void SetItem(LobbyList lobbyList, Lobby lobby)
    {
        lobbyTitleTmp.text = lobby.Name;
        lobbyPlayersTmp.text = lobby.Players.Count + "/" + lobby.MaxPlayers;
        this.lobby = lobby;
        this.lobbyList = lobbyList;
    }

    public void JoinPressed()
    {
        lobbyList.JoinAsync(lobby);
    }
}
