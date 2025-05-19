using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 10f;
    public float damage = 10f;
    public float lifeTime = 5f;

    private Vector3 direction;

    void Start()
    {
        // Direção do projétil (direção para onde o inimigo estava olhando quando o disparou)
        direction = transform.forward;

        Destroy(gameObject, lifeTime); // O projétil será destruído após um tempo determinado
    }

    void Update()
    {
        // Movimento do projétil apenas na direção em que ele foi disparado
        transform.position += direction * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {

         if (other.CompareTag("Props"))
        {
        Destroy(gameObject);
        }

        if (other.CompareTag("Player"))
        {
            PlayerHealth player = other.GetComponent<PlayerHealth>(); // Certifique-se que existe esse script
            if (player != null)
            {
                player.TakeDamage(damage); // Aplica dano ao jogador
            }

            Destroy(gameObject); // Destroi o projétil após atingir o jogador
        }
    }
}
