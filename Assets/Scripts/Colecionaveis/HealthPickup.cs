using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public float healAmount = 25f;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.Heal(healAmount);
            }

            Destroy(gameObject); // Remove o item após ser pego
        }
    }
}
