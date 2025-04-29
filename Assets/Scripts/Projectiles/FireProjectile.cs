using UnityEngine;

public class FireProjectile : MonoBehaviour
{
    public float damage = 10f;
    public float explosionRadius = 3f;
    public float speed = 20f;
    public float lifeTime = 3f;
    public GameObject explosionEffectPrefab;  // Prefab do efeito de explosão

    void Start()
    {
        Destroy(gameObject, lifeTime);  // Destroi o projétil após o tempo de vida
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);  // Movimenta o projétil para frente
    }

    void OnTriggerEnter(Collider other)
    {
        // Aplica dano na área de explosão
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in hitColliders)
        {
            if (hit.CompareTag("Enemy"))
            {
                // Verifica se o inimigo tem o componente EnemyAI antes de tentar aplicar dano
                EnemyAI enemyAI = hit.GetComponent<EnemyAI>();
                if (enemyAI != null)
                {
                    enemyAI.TakeDamage(damage);  // Aplica dano no inimigo
                }
            }
        }

        // Instancia o efeito de explosão na posição do projétil ao ser destruído
        if (explosionEffectPrefab != null)
        {
            // Instancia o efeito de explosão na posição atual do projétil
            GameObject explosion = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            Destroy(explosion, 2f);  // Destroi o efeito de explosão após 2 segundos
        }

        Destroy(gameObject);  // Destroi o projétil após a colisão
    }
}
