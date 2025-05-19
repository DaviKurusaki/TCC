using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public Animator animator;              // Arraste seu Animator aqui no Inspector
    public float speed = 7f;               // Velocidade normal do jogador
    public float dashSpeed = 20f;          // Velocidade durante o dash
    public float dashDuration = 1f;        // Duração do dash
    public float dashCooldown = 1f;        // Tempo mínimo entre dashes
    public Transform modelTransform;       // Arraste o GameObject visual aqui

    private float dashTimeLeft = 0f;
    private float dashCooldownTimer = 0f;
    private bool isDashing = false;
    private Vector3 dashDirection;
    private Rigidbody rb;
    private Vector3 moveDirection;

    public bool canMove = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!canMove)
        {
            animator.SetBool("IsRunning", false);
            return;
        }

        // Captura input
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        moveDirection = new Vector3(moveX, 0f, moveZ).normalized;

        // Atualiza animação de corrida
        bool isMoving = moveDirection.magnitude > 0.1f;
        animator.SetBool("IsRunning", isMoving);

        // Atualiza cooldown do dash
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
            // Inicia o dash
            if (Input.GetKeyDown(KeyCode.LeftShift) && isMoving && dashCooldownTimer <= 0f)
            {
                StartDash();
            }
        }
    }

    void FixedUpdate()
    {
        if (isDashing)
        {
            Vector3 dashVelocity = dashDirection * dashSpeed;
            dashVelocity.y = rb.velocity.y; // Mantém a gravidade
            rb.velocity = dashVelocity;
        }
        else
        {
            Vector3 velocity = moveDirection * speed;
            velocity.y = rb.velocity.y; // Mantém a gravidade
            rb.velocity = velocity;
        }
    }

    private void StartDash()
    {
        isDashing = true;
        dashTimeLeft = dashDuration;
        dashCooldownTimer = dashCooldown;

        rb.detectCollisions = false;               // Desliga colisões temporariamente
        dashDirection = moveDirection;             // Mantém a direção atual
        animator.SetBool("IsDashing", true);       // Liga animação de dash
        // modelTransform.localPosition = new Vector3(0f, -1.5f, 0f); // Visual opcional
    }

    private void EndDash()
    {
        modelTransform.localPosition = Vector3.zero; // Reseta posição visual (opcional)
        isDashing = false;
        rb.detectCollisions = true;
        animator.SetBool("IsDashing", false);        // Desliga animação de dash
    }
}
