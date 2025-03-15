using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ProjectileLauncher : NetworkBehaviour
{
    [SerializeField] GameObject serverProjectilePrefab;
    [SerializeField] GameObject clientProjectilePrefab;

    [SerializeField] float projectileSpeed = 3;
    [SerializeField] float coolTime = 1;
    [SerializeField] float initialDistance = 1;

    float coolTimer = 1;

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

        coolTimer+=Time.deltaTime;
    }

    public bool IsAvailable()
    {
        return coolTimer >= coolTime;
    }

    public void Attack(Vector3 targetPosition)
    {
        if (coolTimer<coolTime) { return; }

        coolTimer = 0;

        Vector3 direction = (targetPosition - transform.position).normalized;
        Vector3 spawnPoint = direction * initialDistance + transform.position;
        spawnPoint.y = spawnPoint.y + 1;

        StartCoroutine(FireAfterDelay(spawnPoint,direction));
    }

    IEnumerator FireAfterDelay(Vector3 spawnPoint, Vector3 direction)
    {
        yield return new WaitForSeconds(0.5f);

        SpawnDummyProjectile(spawnPoint, direction);
        FireServerRpc(spawnPoint, direction);
    }

    [ServerRpc]
    void FireServerRpc(Vector3 spawnPoint, Vector3 direction)
    {
        GameObject projectile = Instantiate(serverProjectilePrefab, spawnPoint, Quaternion.identity);
        projectile.GetComponent<DealDamageOnContact>().SetOwner(OwnerClientId);
        Physics.IgnoreCollision(GetComponent<Collider>(), projectile.GetComponent<Collider>());
        if (projectile.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.linearVelocity = direction * projectileSpeed;
        }

        FireClientRpc(spawnPoint, direction);
    }

    [ClientRpc]
    void FireClientRpc(Vector3 spawnPoint, Vector3 direction)
    {
        if (IsOwner) return;

        SpawnDummyProjectile(spawnPoint, direction);
    }

    void SpawnDummyProjectile(Vector3 spawnPoint, Vector3 direction)
    {
        GameObject projectile = Instantiate(clientProjectilePrefab,spawnPoint, Quaternion.identity);

        Physics.IgnoreCollision(GetComponent<Collider>(), projectile.GetComponent<Collider>());
        if (projectile.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.linearVelocity = direction * projectileSpeed;
        }
    }
}
