using UnityEngine;
using System.Collections.Generic;

public class LightningProjectile : MonoBehaviour
{
    public float damage = 10f;
    public float chainRange = 4f;
    public int chainsLeft = 3;
    public float speed = 20f;
    public float lifeTime = 3f;
    public GameObject lightningEffectPrefab;
    public GameObject projectilePrefab;

    private Transform target;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;

        // Aplica dano
        EnemyAI enemy = other.GetComponent<EnemyAI>();
        if (enemy != null)
            enemy.TakeDamage(damage);

        // Efeito visual
        if (lightningEffectPrefab != null)
            Instantiate(lightningEffectPrefab, other.transform.position + Vector3.up, Quaternion.identity);

        // Procura próximo alvo e cria novo projétil
        if (chainsLeft > 0)
        {
            Transform nextTarget = FindNextTarget(other.transform);
            if (nextTarget != null)
            {
                Vector3 spawnPos = other.transform.position + Vector3.up * 0.5f;

                GameObject newProj = Instantiate(projectilePrefab, spawnPos, Quaternion.LookRotation(nextTarget.position - spawnPos));
                LightningProjectile projScript = newProj.GetComponent<LightningProjectile>();
                projScript.damage = damage * 0.5f;
                projScript.chainsLeft = chainsLeft - 1;
                projScript.projectilePrefab = projectilePrefab; // mantém referência
                projScript.lightningEffectPrefab = lightningEffectPrefab;
            }
        }

        Destroy(gameObject); // destrói o projétil atual
    }

    Transform FindNextTarget(Transform currentTarget)
    {
        Collider[] hits = Physics.OverlapSphere(currentTarget.position, chainRange);
        Transform closest = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Enemy") && hit.transform != currentTarget)
            {
                float dist = Vector3.Distance(currentTarget.position, hit.transform.position);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closest = hit.transform;
                }
            }
        }

        return closest;
    }
}
