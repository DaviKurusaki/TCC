using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Status do Inimigo")]
    public float health = 100f;
    public int xpValue = 25;

    [Header("Detecção e Ataque")]
    public float detectionRange = 15f;
    public float attackRange = 7f;
    public float fireRate = 1.5f;

    [Header("Projétil")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 15f;

    [Header("Drop de Vida")]
    public GameObject healthPickupPrefab;
    [Range(0f, 1f)]
    public float dropChance = 0.3f; // 30% de chance

    [Header("Knockback")]
    public float knockbackForce = 5f;
    public float knockbackDuration = 1f;

    private Transform player;
    private NavMeshAgent agent;
    private Rigidbody rb;
    private float fireCooldown = 0f;
    private bool isKnockedBack = false;
    private float knockbackTimer = 0f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (player == null) return;

        // Se está em knockback, contar o tempo e não atacar
        if (isKnockedBack)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0f)
            {
                isKnockedBack = false;
                agent.enabled = true;
            }
            return; // Impede ações durante o knockback
        }

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
            agent.isStopped = true;
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
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Rigidbody rbProj = projectile.GetComponent<Rigidbody>();

            if (rbProj != null)
            {
                rbProj.velocity = firePoint.forward * projectileSpeed;
            }

            fireCooldown = fireRate;
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0f)
        {
            FindObjectOfType<UIManager>().AddScore(1);

            if (Random.value <= dropChance && healthPickupPrefab != null)
            {
                Instantiate(healthPickupPrefab, transform.position + Vector3.up * 1f, Quaternion.identity);
            }

            FindObjectOfType<PlayerXP>().GainXP(xpValue);

            Destroy(gameObject);
        }
        else
        {
            ApplyKnockback(); // Aplica knockback quando ainda está vivo
        }
    }

    void ApplyKnockback()
    {
        if (rb == null || player == null) return;

        isKnockedBack = true;
        knockbackTimer = knockbackDuration;

        agent.enabled = false; // Desativa NavMeshAgent durante o knockback

        Vector3 direction = (transform.position - player.position).normalized;
        direction.y = 0f;

        rb.velocity = Vector3.zero;
        rb.AddForce(direction * knockbackForce, ForceMode.Impulse);
    }
}
