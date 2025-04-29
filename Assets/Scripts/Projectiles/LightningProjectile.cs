using UnityEngine;
using System.Collections.Generic;

public class LightningProjectile : MonoBehaviour
{
    public float damage = 10f;
    public float chainRange = 4f;
    public int maxChains = 3;
    public float speed = 20f;
    public float lifeTime = 3f;
    public GameObject lightningEffectPrefab;

    private List<Transform> alreadyHit = new List<Transform>();

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
        if (other.CompareTag("Enemy"))
        {
            alreadyHit.Add(other.transform);
            StartCoroutine(ChainLightning(other.transform, maxChains));
            Destroy(gameObject); // destrói o projétil base
        }
    }

    System.Collections.IEnumerator ChainLightning(Transform currentTarget, int chainsLeft)
    {
        if (chainsLeft <= 0 || currentTarget == null)
            yield break;

        EnemyAI enemy = currentTarget.GetComponent<EnemyAI>();
        if (enemy != null)
            enemy.TakeDamage(damage);

        // Cria o efeito visual
        if (lightningEffectPrefab != null)
        {
            Instantiate(lightningEffectPrefab, currentTarget.position + Vector3.up, Quaternion.identity);
        }

        yield return new WaitForSeconds(0.2f); // pequeno delay visual

        Collider[] hitColliders = Physics.OverlapSphere(currentTarget.position, chainRange);
        Transform nextTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider hit in hitColliders)
        {
            if (hit.CompareTag("Enemy") && !alreadyHit.Contains(hit.transform))
            {
                float dist = Vector3.Distance(currentTarget.position, hit.transform.position);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    nextTarget = hit.transform;
                }
            }
        }

        if (nextTarget != null)
        {
            alreadyHit.Add(nextTarget);
            StartCoroutine(ChainLightning(nextTarget, chainsLeft - 1));
        }
    }
}
