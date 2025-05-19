using UnityEngine;
using UnityEngine.AI;

public class FireBossAI : MonoBehaviour, IDamageable
{
    private Transform player;
    private NavMeshAgent agent;
    private Rigidbody rb;

    [Header("Status do Boss")]
    public float health = 500f;
    public float enragedThreshold = 200f;
    private bool isEnraged = false;

    public int xpValue = 100;
    public float movementSpeed = 3f;

    [Header("Ataques")]
    public float detectionRange = 20f;
    public float attackRange = 10f;
    public float fireRate = 2f;
    public GameObject fireballPrefab;
    public Transform firePoint;
    public float projectileSpeed = 20f;

    [Header("Chuva de Fogo")]
    public GameObject meteorPrefab;
    public int meteorCount = 8;
    public float meteorRadius = 5f;
    public float meteorCooldown = 10f;
    private float meteorTimer;
    public float height = 20f;

    [Header("Explosão Flamejante")]
    public GameObject explosionEffect;
    public float explosionRadius = 6f;
    public float explosionDamage = 30f;
    private bool explosionUsed = false;

    [Header("Knockback")]
    public float knockbackForce = 8f;
    public float knockbackDuration = 1.2f;

    private float fireCooldown;
    private bool isKnockedBack = false;
    private float knockbackTimer;
    private Renderer bossRenderer;
    private Color originalColor;

    [Header("Congelamento")]
    public float freezeDuration = 3f;
    private bool isFrozen = false;
    private float freezeTimer;
    private Color frozenColor = Color.cyan;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        agent.speed = movementSpeed;
        fireCooldown = 0f;
        meteorTimer = meteorCooldown;

        bossRenderer = GetComponentInChildren<Renderer>();
        if (bossRenderer != null)
            originalColor = bossRenderer.material.color;
    }

    void Update()
    {
        if (player == null) return;

        if (isFrozen)
    {
        freezeTimer -= Time.deltaTime;
        if (freezeTimer <= 0f)
        {
            isFrozen = false;
            agent.isStopped = false;

            if (bossRenderer != null)
                bossRenderer.material.color = originalColor;
        }
        return; // cancela ações enquanto congelado
    }


        if (isKnockedBack)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0f)
            {
                isKnockedBack = false;
                agent.enabled = true;
                if (bossRenderer) bossRenderer.material.color = originalColor;
            }
            return;
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

        // Chuva de fogo
        meteorTimer -= Time.deltaTime;
        if (meteorTimer <= 0f)
        {
            CastMeteorShower();
            meteorTimer = meteorCooldown;
        }

        fireCooldown -= Time.deltaTime;

        // Explosão flamejante
        if (!explosionUsed && health <= enragedThreshold)
        {
            CastExplosion();
            explosionUsed = true;
            isEnraged = true;
            agent.speed = movementSpeed + 1.5f; // Fica mais rápido
        }
    }


    public void Freeze()
    {
        if (isFrozen) return;

        isFrozen = true;
        freezeTimer = freezeDuration;
        agent.isStopped = true;

        if (rb != null)
            rb.velocity = Vector3.zero;

        if (bossRenderer != null)
            bossRenderer.material.color = frozenColor;
    }

    void LookAtPlayer()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0;
        Quaternion rot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 10f);
    }

    void AttackPlayer()
    {
        if (fireCooldown <= 0f)
        {
            
            GameObject fireball = Instantiate(fireballPrefab, firePoint.position, firePoint.rotation);
            Rigidbody rbProj = fireball.GetComponent<Rigidbody>();
            if (rbProj != null)
                rbProj.velocity = firePoint.forward * projectileSpeed;

            fireCooldown = fireRate;
        }
    }

    void CastMeteorShower()
    {
        for (int i = 0; i < meteorCount; i++)
        {
            Vector3 offset = Random.insideUnitCircle * meteorRadius;
            Vector3 spawnPos = player.position + new Vector3(offset.x, height, offset.y);
            Instantiate(meteorPrefab, spawnPos, Quaternion.identity);
        }
    }

    void CastExplosion()
    {
        if (explosionEffect != null)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var hit in hitColliders)
        {
            if (hit.CompareTag("Player"))
            {
                IDamageable dmg = hit.GetComponent<IDamageable>();
                if (dmg != null)
                    dmg.TakeDamage(explosionDamage);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0f)
        {
            FindObjectOfType<PlayerXP>().GainXP(xpValue);
            Destroy(gameObject);
        }
        else
        {
            ApplyKnockback();
        }
    }

    void ApplyKnockback()
    {
        isKnockedBack = true;
        knockbackTimer = knockbackDuration;
        agent.enabled = false;

        if (rb != null)
            rb.velocity = -transform.forward * knockbackForce;

        if (bossRenderer != null)
            bossRenderer.material.color = Color.red;
    }
}
