using UnityEngine;
using UnityEngine.AI;

public class IceBossAI : MonoBehaviour, IDamageable
{
    private Transform player;
    private NavMeshAgent agent;
    private Rigidbody rb;

    [Header("Status do Boss de Gelo")]
    public float health = 600f;
    public float enragedThreshold = 250f;
    private bool isEnraged = false;

    public int xpValue = 120;
    public float movementSpeed = 2.5f;

    [Header("Detecção e Ataque")]
    public float detectionRange = 18f;
    public float attackRange = 12f;
    public float iceRate = 2f;
    public GameObject iceShardPrefab;
    public Transform icePoint;
    public float projectileSpeed = 18f;

    [Header("Chuva de Estalactites")]
    public GameObject spikePrefab;
    public int spikeCount = 6;
    public float spikeRadius = 6f;
    public float spikeCooldown = 12f;
    private float spikeTimer;

    [Header("Explosão Congelante")]
    public GameObject iceExplosionEffect;
    public float explosionRadius = 6f;
    public float explosionDamage = 25f;
    public float slowDuration = 3f;
    private bool explosionUsed = false;

    [Header("Knockback")]
    public float knockbackForce = 7f;
    public float knockbackDuration = 1.1f;

    private float iceCooldown;
    private bool isKnockedBack = false;
    private float knockbackTimer;
    private Renderer bossRenderer;
    private Color originalColor;

    [Header("Congelamento (Recebido)")]
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
        iceCooldown = 0f;
        spikeTimer = spikeCooldown;

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
            return;
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

        // Chuva de gelo
        spikeTimer -= Time.deltaTime;
        if (spikeTimer <= 0f)
        {
            CastIceSpikes();
            spikeTimer = spikeCooldown;
        }

        iceCooldown -= Time.deltaTime;

        // Explosão congelante ao ficar enfurecido
        if (!explosionUsed && health <= enragedThreshold)
        {
            CastIceExplosion();
            explosionUsed = true;
            isEnraged = true;
            agent.speed = movementSpeed + 1f;
        }
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
        if (iceCooldown <= 0f)
        {
            GameObject shard = Instantiate(iceShardPrefab, icePoint.position, icePoint.rotation);
            Rigidbody rbProj = shard.GetComponent<Rigidbody>();
            if (rbProj != null)
                rbProj.velocity = icePoint.forward * projectileSpeed;

            iceCooldown = iceRate;
        }
    }

    void CastIceSpikes()
    {
        for (int i = 0; i < spikeCount; i++)
        {
            Vector2 offset = Random.insideUnitCircle * spikeRadius;
            Vector3 spawnPos = player.position + new Vector3(offset.x, 0.5f, offset.y);
            Instantiate(spikePrefab, spawnPos, Quaternion.identity);
        }
    }

   void CastIceExplosion()
{
    if (iceExplosionEffect != null)
        Instantiate(iceExplosionEffect, transform.position, Quaternion.identity);

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

    void ApplyKnockback()
    {
        isKnockedBack = true;
        knockbackTimer = knockbackDuration;
        agent.enabled = false;

        if (rb != null)
            rb.velocity = -transform.forward * knockbackForce;

        if (bossRenderer != null)
            bossRenderer.material.color = Color.cyan;
    }
}
