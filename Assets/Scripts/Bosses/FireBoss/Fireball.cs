using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float damage = 20f;
    public float lifetime = 5f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Primeiro tenta usar a interface IDamageable
            IDamageable dmg = other.GetComponent<IDamageable>();
            if (dmg != null)
            {
                dmg.TakeDamage(damage);
            }
            else
            {
                // Se não tiver a interface, tenta usar PlayerHealth direto
                PlayerHealth player = other.GetComponent<PlayerHealth>();
                if (player != null)
                    player.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }
}
