using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class StatDisplayer : NetworkBehaviour
{
    [SerializeField] Health health;
    [SerializeField] MagicPoint magicPoint;
    [SerializeField] Image healthBarImage;
    [SerializeField] Image magicBarImage;

    [SerializeField] TMP_Text userNameTmp;
    public NetworkVariable<FixedString128Bytes> userName = new NetworkVariable<FixedString128Bytes>();
    
    public override void OnNetworkSpawn()
    {
        if (!IsClient) { return; }

        if (health!=null)
        {
            health.currentHealth.OnValueChanged += HandleHealthChange;
        }
        if (magicPoint != null)
        {
            magicPoint.currentMagic.OnValueChanged += HandleMagicPointChange;
        }

        if (IsServer)
        {
            userName.Value = ServerSingleton.Instance.clientIdToUserData[OwnerClientId].userName;
        }
        if (userNameTmp != null)
        {
            userName.OnValueChanged += HandleUsernameChange;
            UpdateName();
        }
    }

    private void HandleUsernameChange(FixedString128Bytes previousValue, FixedString128Bytes newValue)
    {
        UpdateName();
    }

    void UpdateName()
    {
        string newName = userName.Value.ToString();
        if (newName.Contains("#"))
        {
            newName = newName.Substring(0, newName.LastIndexOf("#"));
        }
        userNameTmp.text = newName;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsClient) { return; }

        if (health != null)
        {
            health.currentHealth.OnValueChanged -= HandleHealthChange;
        }
        if (magicPoint != null)
        {
            magicPoint.currentMagic.OnValueChanged -= HandleMagicPointChange;
        }
        if (userNameTmp != null && userName!=null)
        {
            userName.OnValueChanged -= HandleUsernameChange;
        }

    }

    void HandleHealthChange(int oldHealth, int newHealth)
    {
        healthBarImage.fillAmount = newHealth / (float)health.maxHealth;
    }
    void HandleMagicPointChange(float oldMagic, float newMagic)
    {
        magicBarImage.fillAmount = newMagic / magicPoint.maxMagicPoint;
    }

}
