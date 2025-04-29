using UnityEngine;

public class HomingProjectile : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 5f;
    public float areaRadius = 5f;
    private float damage;  // Agora o dano será ajustado dinamicamente
    private Transform target;
    private string projectileType;

    public void SetTarget(Transform newTarget, string type)
    {
        target = newTarget;
        projectileType = type;
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 direction = (target.position - transform.position).normalized;
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (projectileType == "Fire" || projectileType == "Ice")
            {
                ApplyAreaDamage(transform.position);
            }
            else
            {
                EnemyAI enemy = other.GetComponent<EnemyAI>();
                if (enemy != null)
                {
                    // Aplique o dano do projétil somado ao dano do objeto do inimigo
                    enemy.TakeDamage(damage);  
                }
            }
            Destroy(gameObject);
        }
    }

    void ApplyAreaDamage(Vector3 impactPosition)
    {
        Collider[] hitColliders = Physics.OverlapSphere(impactPosition, areaRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                EnemyAI enemy = hitCollider.GetComponent<EnemyAI>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage); 
                }
            }
        }
    }

    // Método para definir o dano dinamicamente
    public void SetDamage(float newDamage)
    {
        damage = newDamage;
    }

    // Método para definir a velocidade dinamicamente
    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
}
