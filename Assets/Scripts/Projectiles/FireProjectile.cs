using UnityEngine;

public class FireProjectile : MonoBehaviour
{
    public float damage = 10f;
    public float explosionRadius = 2f;
    public float speed = 20f;
    public float lifeTime = 3f;
    public GameObject explosionEffectPrefab;

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
        EnemyAI mainTarget = null;

        if (other.CompareTag("Enemy"))
        {
            mainTarget = other.GetComponent<EnemyAI>();
            if (mainTarget != null)
            {
                mainTarget.TakeDamage(damage); // Dano total no alvo direto
            }
        }

        // Aplica dano em área (metade) para os inimigos próximos, exceto o atingido
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in hitColliders)
        {
            if (hit.CompareTag("Enemy") && hit != other)
            {
                EnemyAI enemyAI = hit.GetComponent<EnemyAI>();
                if (enemyAI != null)
                {
                    enemyAI.TakeDamage(damage / 2f); // Dano reduzido em área
                }
            }
        }

        // Efeito de explosão
        if (explosionEffectPrefab != null)
        {
            GameObject explosion = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            Destroy(explosion, 2f);
        }

        Destroy(gameObject);
    }
}
