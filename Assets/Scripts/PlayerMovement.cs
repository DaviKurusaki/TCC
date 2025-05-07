using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public Animator animator;           // Arraste seu Animator aqui no Inspector
    public float speed = 7f;            // Velocidade normal do jogador
    public float dashSpeed = 20f;       // Velocidade durante o dash
    public float dashDuration = 1f;   // Duração do dash
    public float dashCooldown = 1f;     // Tempo mínimo entre dashes
    private float dashTimeLeft = 0f;
    private float dashCooldownTimer = 0f;
    private bool isDashing = false;
    private Vector3 dashDirection;

    public Transform modelTransform;  // arraste o GameObject visual aqui


    private Rigidbody rb;
    private Vector3 moveDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // 1) Ler input de movimento (WASD ou Setas)
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        moveDirection = new Vector3(moveX, 0f, moveZ).normalized;

        // 2) Verificar se o jogador está se movendo e atualizar o parâmetro IsRunning
        bool isMoving = moveDirection.magnitude > 0.1f;
        animator.SetBool("IsRunning", isMoving);  // Atualiza o parâmetro IsRunning no Animator

        // 3) Atualizar timers do dash
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
            // 4) Iniciar dash se pressionar Shift, movimento estiver ativo e cooldown expirado
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
            dashVelocity.y = rb.velocity.y; // Mantém a gravidade funcionando
            rb.velocity = dashVelocity;
        }
        else
        {
            // 5) Movimento normal com velocidade ajustada
            Vector3 velocity = moveDirection * speed;
            velocity.y = rb.velocity.y; // Mantém a gravidade funcionando
            rb.velocity = velocity;
        }
    }

    // Iniciar o dash
    private void StartDash()
    {
      
        isDashing = true;
        dashTimeLeft = dashDuration;
        dashCooldownTimer = dashCooldown;
        

        // Desabilita colisões para não ser empurrado
        rb.detectCollisions = false;

        // Mantém a direção atual de movimento
        dashDirection = moveDirection;
          animator.SetBool("IsDashing", true);   // Quando começa o dash
          modelTransform.localPosition = new Vector3(0f, -1f, 0f); // afunda visualmente
        
    }

    // Finaliza o dash
    private void EndDash()
    {
       
    modelTransform.localPosition = Vector3.zero; // volta ao normal

        isDashing = false;
        rb.detectCollisions = true;
         animator.SetBool("IsDashing", false);  // Quando termina o dash
    }
}
