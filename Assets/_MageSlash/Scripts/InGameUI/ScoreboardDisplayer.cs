using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;

public class ScoreboardDisplayer : MonoBehaviour
{
    [SerializeField] TMP_Text usernameTmp;
    [SerializeField] TMP_Text scoreTmp;

    public ulong clientId;
    public FixedString128Bytes userName;
    public int score;

    public void SetScore(ulong clientId, FixedString128Bytes username, int score)
    {
        this.clientId = clientId;
        this.score = score;
        this.userName = username;

        usernameTmp.text = userName.ToString();
        scoreTmp.text = score.ToString();
    } 
}
