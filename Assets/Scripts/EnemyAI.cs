using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamageable
{
    private PlayerAttack playerAttack;

    [Header("Status do Inimigo")]
    public float health = 100f;
    public int xpValue = 0;

    public float movementSpeed = 3.5f;
    private float originalSpeed;

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
    [Range(0f, 1f)] public float dropChance = 0.3f;

    [Header("Knockback")]
    public float knockbackForce = 5f;
    public float knockbackDuration = 1f;

    private Renderer[] renderers;
    private Color[] originalColors;


    [Header("Frozen")]
    public bool isFrozen = false;
    private float freezeTimer = 0f;

    private Transform player;
    private NavMeshAgent agent;
    private Rigidbody rb;
    private float fireCooldown = 0f;
    private bool isKnockedBack = false;
    private float knockbackTimer = 0f;

    private bool isFlashing = false; // flag para evitar conflito de cores no FlashRed

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        playerAttack = FindObjectOfType<PlayerAttack>();

        if (agent != null)
        {
            agent.speed = movementSpeed;
            originalSpeed = movementSpeed;
        }

        renderers = GetComponentsInChildren<Renderer>();
       originalColors = new Color[renderers.Length];

    for (int i = 0; i < renderers.Length; i++)
    {
        originalColors[i] = renderers[i].material.color;
    }

    }

    void Update()
    {
        if (player == null || isFrozen || isKnockedBack) return;

        float distance = Vector3.Distance(transform.position, player.position);
        fireCooldown -= Time.deltaTime;

        if (distance <= detectionRange)
        {
            if (distance > attackRange)
            {
                agent.SetDestination(player.position);
            }
            else
            {
                agent.ResetPath();
                LookAtPlayer();
                AttackPlayer();
            }
        }
        else
        {
            agent.ResetPath();
        }
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

    void FlashRed()
{
    if (isFlashing || isFrozen) return;

    isFlashing = true;

    foreach (var rend in renderers)
        rend.material.color = Color.red;

    CancelInvoke(nameof(RestoreToBaseColor));
    Invoke(nameof(RestoreToBaseColor), 0.2f);
}


 void RestoreToBaseColor()
{
    for (int i = 0; i < renderers.Length; i++)
    {
        renderers[i].material.color = originalColors[i];
    }

    isFlashing = false;
}



    public void TakeDamage(float damage)
    {
        health -= damage;
        if (!isFrozen)
            FlashRed();

        if (health <= 0f)
        {
            FindObjectOfType<UIManager>()?.AddScore(1);

            if (Random.value <= dropChance && healthPickupPrefab != null)
            {
                Instantiate(healthPickupPrefab, transform.position + Vector3.up * 1f, Quaternion.identity);
            }

            FindObjectOfType<PlayerXP>()?.GainXP(xpValue);

            if (playerAttack != null)
            {
                playerAttack.currentMagicAmmo++;
                playerAttack.UpdateAmmoUI();
            }

            Destroy(gameObject);
        }
        else
        {
            ApplyKnockback();
        }
    }

    void ApplyKnockback()
    {
        if (isFrozen) return;

        isKnockedBack = true;
        knockbackTimer = knockbackDuration;

        if (agent != null)
            agent.ResetPath();

        if (rb != null)
            rb.velocity = Vector3.zero;

        FlashRed();

        Invoke(nameof(EndKnockback), knockbackDuration);
    }

    void EndKnockback()
    {
        isKnockedBack = false;
        RestoreToBaseColor();
    }

    public void Freeze(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(FreezeCoroutine(duration));
    }

  private System.Collections.IEnumerator FreezeCoroutine(float duration)
{
    isFrozen = true;

    if (agent != null)
    {
        originalSpeed = agent.speed;
        agent.isStopped = true;
        agent.speed = 0f;
    }

    foreach (var rend in renderers)
        rend.material.color = Color.cyan;

    yield return new WaitForSeconds(duration);

    isFrozen = false;

    if (agent != null)
    {
        agent.isStopped = false;
        agent.speed = originalSpeed;
    }

        // Aqui, voltar à cor original
        RestoreToBaseColor();
}

    public void Slow(float duration)
    {
        StartCoroutine(SlowCoroutine(duration));
    }

    private System.Collections.IEnumerator SlowCoroutine(float duration)
    {
        if (agent != null)
        {
            float currentSpeed = agent.speed;
            agent.speed = currentSpeed * 0.3f;
            yield return new WaitForSeconds(duration);
            agent.speed = originalSpeed;
        }
    }
}
