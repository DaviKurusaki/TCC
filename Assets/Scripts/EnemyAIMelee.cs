using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyAIMelee : MonoBehaviour, IDamageable
{
    private PlayerAttack playerAttack;

    [Header("Status do Inimigo")]
    public float health = 100f;
    public int xpValue = 0;
    public float attackDamage = 10f;
    public float attackCooldown = 2f;
    public float movementSpeed = 3.5f;

    [Header("Detecção e Ataque")]
    public float detectionRange = 15f;
    public float attackRange = 2f;

    [Header("Drop de Vida")]
    public GameObject healthPickupPrefab;
    [Range(0f, 1f)]
    public float dropChance = 0.3f;

    [Header("Knockback")]
    public float knockbackForce = 5f;
    public float knockbackDuration = 1f;

    private Transform player;
    private NavMeshAgent agent;
    private Rigidbody rb;
    private float attackTimer = 0f;
    private bool isKnockedBack = false;
    private float knockbackTimer = 0f;

    private Renderer[] renderers;
    private Color[] originalColors;

    private bool isFrozen = false;
 
    private float originalSpeed;
    private float freezeTimer = 0f;
    private Coroutine freezeCoroutine;
    private Coroutine slowCoroutine;

    void Start()
    {

        originalSpeed = movementSpeed;

        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        agent.speed = movementSpeed;
        originalSpeed = movementSpeed;
        rb = GetComponent<Rigidbody>();

        renderers = GetComponentsInChildren<Renderer>();
        originalColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
            originalColors[i] = renderers[i].material.color;

        playerAttack = FindObjectOfType<PlayerAttack>();
    }

    void Update()
    {

        if (isFrozen)
        {
            freezeTimer -= Time.deltaTime;
            if (freezeTimer <= 0f)
            {
                isFrozen = false;
                agent.isStopped = false;
                movementSpeed = originalSpeed;
                agent.speed = originalSpeed;
                ResetColors();
            }
            return;
        }

        if (player == null || isFrozen) return;

        if (isKnockedBack)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0f)
            {
                isKnockedBack = false;
                agent.enabled = true;
                ResetColors();
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
        else
        {
            agent.isStopped = true;
        }

        attackTimer -= Time.deltaTime;
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
        if (attackTimer <= 0f)
        {
            PlayerHealth ph = player.GetComponent<PlayerHealth>();
            if (ph != null) ph.TakeDamage(attackDamage);
            attackTimer = attackCooldown;
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (!isFrozen) // Só pisca vermelho se não estiver congelado
        FlashRed();

        if (health <= 0f)
        {
            FindObjectOfType<UIManager>().AddScore(1);
            if (Random.value <= dropChance && healthPickupPrefab != null)
                Instantiate(healthPickupPrefab, transform.position + Vector3.up, Quaternion.identity);
            FindObjectOfType<PlayerXP>().GainXP(xpValue);

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
        isKnockedBack = true;
        knockbackTimer = knockbackDuration;
        if (agent != null) agent.enabled = false;
        if (rb != null) rb.velocity = Vector3.zero;
    }

    void FlashRed()
    {
        foreach (var rend in renderers)
            rend.material.color = Color.red;
    }

    void ResetColors()
    {
        for (int i = 0; i < renderers.Length; i++)
            renderers[i].material.color = originalColors[i];
    }

    // Congelar inimigo por duração
    public void Freeze(float duration)
    {
        isFrozen = true;
        freezeTimer = duration;
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        movementSpeed = 0f;

        foreach (var rend in renderers)
        rend.material.color = Color.cyan; // Azul claro para congelado

        if (freezeCoroutine != null)
            StopCoroutine(freezeCoroutine);

        freezeCoroutine = StartCoroutine(FreezeRoutine(duration));
    }

    private IEnumerator FreezeRoutine(float duration)
    {
        isFrozen = true;
        agent.isStopped = true;
        originalSpeed = agent.speed;
        agent.speed = 0f;

        // Opcional: muda cor para azul para indicar congelamento
        foreach (var rend in renderers)
            rend.material.color = Color.cyan;

        yield return new WaitForSeconds(duration);

        agent.speed = originalSpeed;
        agent.isStopped = false;
        isFrozen = false;
        ResetColors();
    }

    // Lentidão do inimigo por duração
    public void Slow(float duration)
    {
        if (slowCoroutine != null)
            StopCoroutine(slowCoroutine);

        slowCoroutine = StartCoroutine(SlowRoutine(duration));
    }

    private IEnumerator SlowRoutine(float duration)
    {
        float slowedSpeed = originalSpeed * 0.5f; // Exemplo: reduz para 50%
        agent.speed = slowedSpeed;

        // Opcional: muda cor para azul claro para indicar lentidão
        foreach (var rend in renderers)
            rend.material.color = Color.blue;

        yield return new WaitForSeconds(duration);

        agent.speed = originalSpeed;
        ResetColors();
    }
}
