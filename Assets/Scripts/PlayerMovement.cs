using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody rb;
    private Vector3 moveDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Move();
    }

    void Move()
    {
        float moveX = Input.GetAxis("Horizontal"); // A/D ou Esquerda/Direita
        float moveZ = Input.GetAxis("Vertical");   // W/S ou Cima/Baixo

        moveDirection = new Vector3(moveX, 0, moveZ).normalized;
    }

    void FixedUpdate()
    {
        rb.velocity = moveDirection * speed;
    }
}
