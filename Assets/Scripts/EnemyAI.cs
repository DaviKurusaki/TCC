using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public float health = 100f;
    public float detectionRange = 15f;
    public float attackRange = 7f;
    public float fireRate = 1.5f;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 15f;

    private Transform player;
    private NavMeshAgent agent;
    private float fireCooldown = 0f;
    public int xpValue = 25;


    public GameObject healthPickupPrefab;
[Range(0f, 1f)]
public float dropChance = 0.3f; // 30% de chance


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectionRange)
        {
            if (distance > attackRange)
            {
                agent.isStopped = false;
                agent.SetDestination(player.position);
            }
            else    
            {
                agent.isStopped = true;
                LookAtPlayer();
                AttackPlayer();
            }
        }
        else
        {
            agent.isStopped = true; // Para caso o jogador fuja da detecção
        }

        fireCooldown -= Time.deltaTime;
    }

    void LookAtPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
    }

    void AttackPlayer()
    {
        if (fireCooldown <= 0f)
        {
            // Criar o projétil na posição do firePoint
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();

            if (rb != null)
            {
                // A direção do projétil será dada pela rotação do firePoint
                rb.velocity = firePoint.forward * projectileSpeed; // Usa a direção do firePoint
            }

            fireCooldown = fireRate;
        }
    }

   public void TakeDamage(float damage)
{
    health -= damage;
    if (health <= 0f)
    {
        // Adiciona um ponto de score quando o inimigo morre
        FindObjectOfType<UIManager>().AddScore(1);

        // Sorteia se vai dropar a vida
        if (Random.value <= dropChance && healthPickupPrefab != null)
        {
            Instantiate(healthPickupPrefab, transform.position + Vector3.up * 1f, Quaternion.identity);
        }

        // Aqui não chamamos mais o evento de level up. Apenas ganha XP.
        FindObjectOfType<PlayerXP>().GainXP(xpValue);

        Destroy(gameObject); // Destroi o inimigo
    }
}

}
