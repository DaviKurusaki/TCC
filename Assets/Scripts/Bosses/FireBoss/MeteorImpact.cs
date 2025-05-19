using UnityEngine;

public class MeteorImpact : MonoBehaviour
{
    public float damage = 30f;
    public float explosionRadius = 5f;
    public GameObject explosionEffectPrefab;
    public float lifetime = 8f;

    void Start()
    {
        Destroy(gameObject, lifetime); // segurança caso não colida
    }

    void OnTriggerEnter(Collider other)
    {
        // Explosão em área
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in hits)
        {
           if (hit.CompareTag("Player"))
            {
                PlayerHealth player = hit.GetComponent<PlayerHealth>();
                if (player != null)
                    player.TakeDamage(damage);
            }

        }

        // Instancia efeito visual de explosão
        if (explosionEffectPrefab != null)
        {
            GameObject explosion = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            Destroy(explosion, 3f);
        }

        Destroy(gameObject);
    }
}
