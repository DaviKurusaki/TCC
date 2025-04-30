using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 7f;                // Velocidade normal

    [Header("Dash Settings")]
    public float dashSpeed = 20f;           // Velocidade durante o dash
    public float dashDuration = 0.2f;       // Duração do dash em segundos
    public float dashCooldown = 1f;         // Tempo mínimo entre dashes
    private float dashTimeLeft = 0f;
    private float dashCooldownTimer = 0f;
    private bool isDashing = false;
    private Vector3 dashDirection;

    [Header("Dust Particle")]
    public ParticleSystem dashDustPrefab;   // Prefab da partícula de poeira
    public Vector3 dustOffset = new Vector3(0f, 0.1f, 0f);

    private Rigidbody rb;
    private Vector3 moveDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // 1) Ler input de movimento
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        moveDirection = new Vector3(moveX, 0f, moveZ).normalized;

        // 2) Atualizar timers
        if (dashCooldownTimer > 0f)
            dashCooldownTimer -= Time.deltaTime;

        if (isDashing)
        {
            dashTimeLeft -= Time.deltaTime;
            if (dashTimeLeft <= 0f)
                EndDash();
        }
        else
        {
            // 3) Iniciar dash se Shift e tiver direção e cooldown zerado
            if (Input.GetKeyDown(KeyCode.LeftShift)
                && moveDirection.sqrMagnitude > 0.01f
                && dashCooldownTimer <= 0f)
            {
                StartDash();
            }
        }
    }

    void FixedUpdate()
    {
        if (isDashing)
        {
            rb.velocity = dashDirection * dashSpeed;
        }
        else
        {
            rb.velocity = moveDirection * speed;
        }
    }

    private void StartDash()
    {
        isDashing = true;
        dashTimeLeft = dashDuration;
        dashCooldownTimer = dashCooldown;

        // Desabilita colisões para não ser empurrado
        rb.detectCollisions = false;

        // Mantém a direção atual de movimento
        dashDirection = moveDirection;

        // Gera a partícula de poeira ATRÁS do jogador
        if (dashDustPrefab != null)
        {
            Vector3 behindOffset = -dashDirection * dustOffset.z 
                                   + Vector3.up * dustOffset.y;
            Vector3 spawnPos = transform.position + behindOffset;

            ParticleSystem dust = Instantiate(dashDustPrefab, spawnPos, Quaternion.identity);
            dust.transform.forward = -dashDirection;
            var main = dust.main;
            dust.Play();
            Destroy(dust.gameObject, main.duration + main.startLifetime.constantMax);
        }
    }

    private void EndDash()
    {
        isDashing = false;

        // Reabilita colisões
        rb.detectCollisions = true;
    }
}
