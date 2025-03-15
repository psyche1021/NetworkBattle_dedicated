using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{

    public int maxHealth = 100;
    public NetworkVariable<int> currentHealth = new NetworkVariable<int>();

    public bool isDead;

    public Action<Health> OnDie;
    public static Action<ulong, int> OnScored;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) {  return; }

        currentHealth.Value = maxHealth;
    }

    public void TakeDamage(int damage, ulong clientId)
    {
        ModifyHealth(-damage, clientId);
    }

    public void RestoreHealth(int heal, ulong clientId)
    {
        ModifyHealth(heal, clientId);
    }

    void ModifyHealth(int value, ulong clientId)
    {
        if (isDead) { return; }
        int newHealth = currentHealth.Value + value;
        currentHealth.Value = Mathf.Clamp(newHealth, 0, maxHealth);

        if (currentHealth.Value==0)
        {
            isDead = true;

            if (GetComponent<PlayerController>() != null)
            {
                OnScored?.Invoke(clientId, 10);
            }
            else if (GetComponent<Monster>() != null)
            {
                OnScored?.Invoke(clientId, 1);
            }

            OnDie?.Invoke(this);

        }
    }
}
