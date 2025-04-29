using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    private UIManager ui;

    void Start()
    {
        currentHealth = maxHealth;
        ui = FindObjectOfType<UIManager>();
        ui.UpdateHealth(currentHealth, maxHealth);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        ui.UpdateHealth(currentHealth, maxHealth);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        ui.UpdateHealth(currentHealth, maxHealth);
    }

    public void IncreaseMaxHealth(float amount)
    {
        maxHealth += amount;
        currentHealth = maxHealth;  // Opcional: Restaurar saúde máxima ao aumentar
        ui.UpdateHealth(currentHealth, maxHealth);
    }

    void Die()
    {
        Debug.Log("Player morreu!");
        Destroy(gameObject);
        // Aqui você pode reiniciar, mostrar game over, etc.
    }
}
