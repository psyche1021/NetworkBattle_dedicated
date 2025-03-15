using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Netcode;
using UnityEngine;

public class Scoreboard : NetworkBehaviour
{
    [SerializeField] Transform scoreboardParent;
    [SerializeField] ScoreboardDisplayer scoreboardDisplayerPrefab;

    NetworkList<ScoreboardData> scoreboardDatas = new NetworkList<ScoreboardData>();
    
    List<ScoreboardDisplayer> scoreboardDisplayers = new List<ScoreboardDisplayer>();

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            scoreboardDatas.OnListChanged += HandleScoreboardsChanged;

            foreach (ScoreboardData data in scoreboardDatas)
            {
                HandleScoreboardsChanged(new NetworkListEvent<ScoreboardData>
                {
                    Type = NetworkListEvent<ScoreboardData>.EventType.Add,
                    Value = data
                });
            }
        }

        if (IsServer)
        {
            PlayerController[] players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
            foreach (PlayerController player in players)
            {
                HandlePlayerSpawned(player);
            }

            PlayerController.OnPlayerSpawn += HandlePlayerSpawned;
            PlayerController.OnPlayerDespawn += HandlePlayerDespawned;

            Health.OnScored += HandleScoreGained;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            scoreboardDatas.OnListChanged -= HandleScoreboardsChanged;
        }

        if (IsServer)
        {
            PlayerController.OnPlayerSpawn -= HandlePlayerSpawned;
            PlayerController.OnPlayerDespawn -= HandlePlayerDespawned;

            Health.OnScored -= HandleScoreGained;

        }
    }

    private void HandlePlayerSpawned(PlayerController player)
    {
        int i = 0;
        for (; i < scoreboardDatas.Count; i++)
        {
            if (scoreboardDatas[i].clientId == player.OwnerClientId)
            {
                break;
            }
        }
        if (i >= scoreboardDatas.Count)
        {
            scoreboardDatas.Add(new ScoreboardData
            {
                clientId = player.OwnerClientId,
                userName = ServerSingleton.Instance.clientIdToUserData[player.OwnerClientId].userName,
                score = 0
            });
        }
    }

    private void HandlePlayerDespawned(PlayerController player)
    {
        if (scoreboardDatas == null) return;
        if (!player.GetComponent<Health>().isDead)
        {
            foreach (ScoreboardData data in scoreboardDatas)
            {
                if (data.clientId == player.OwnerClientId)
                {
                    scoreboardDatas.Remove(data);
                    break;
                }
            }
        }
    }

    private void HandleScoreboardsChanged(NetworkListEvent<ScoreboardData> changeEvent)
    {
        switch (changeEvent.Type)
        {
            case NetworkListEvent<ScoreboardData>.EventType.Add:
                {
                    if (!scoreboardDisplayers.Any(x => x.clientId == changeEvent.Value.clientId))
                    {
                        ScoreboardDisplayer displayer = Instantiate(scoreboardDisplayerPrefab, scoreboardParent);
                        displayer.SetScore(
                            changeEvent.Value.clientId,
                            changeEvent.Value.userName,
                            changeEvent.Value.score);

                        scoreboardDisplayers.Add(displayer);
                    }
                    break;
                }
            case NetworkListEvent<ScoreboardData>.EventType.Remove:
                {
                    ScoreboardDisplayer displayer =
                        scoreboardDisplayers.FirstOrDefault(x => x.clientId == changeEvent.Value.clientId);
                    if (displayer != null)
                    {
                        scoreboardDisplayers.Remove(displayer);
                        Destroy(displayer.gameObject);
                    }
                    break;
                }
            case NetworkListEvent<ScoreboardData>.EventType.Value:
                {
                    ScoreboardDisplayer displayer =
                        scoreboardDisplayers.FirstOrDefault(x => x.clientId == changeEvent.Value.clientId);
                    if (displayer !=null)
                    {
                        displayer.SetScore( changeEvent.Value.clientId,
                            changeEvent.Value.userName,
                            changeEvent.Value.score); 
                    }
                    break;
                }
        }
    }

    private void HandleScoreGained(ulong clientId, int score)
    {
        for (int i = 0; i < scoreboardDatas.Count; i++)
        {
            if (scoreboardDatas[i].clientId == clientId)
            {
                scoreboardDatas[i] = new ScoreboardData
                {
                    clientId = clientId,
                    userName = scoreboardDatas[i].userName,
                    score = scoreboardDatas[i].score + score
                };
                break;
            }
        }
    }


}
