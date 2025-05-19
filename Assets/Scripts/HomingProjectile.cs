using UnityEngine;

public class HomingProjectile : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 5f;
    public float areaRadius = 2f;

    private float damage;
    private Transform target;
    private string projectileType;
    private Vector3 straightDirection = Vector3.zero;
    private bool directionInitialized = false;

    public void SetTarget(Transform newTarget, string type)
    {
        target = newTarget;
        projectileType = type;
    }

    public void SetStraightDirection(Vector3 direction)
    {
        straightDirection = direction.normalized;
    }

    public void SetDamage(float newDamage)
    {
        damage = newDamage;
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            transform.Translate(direction * speed * Time.deltaTime, Space.World);
            directionInitialized = true;
        }
        else if (straightDirection != Vector3.zero)
        {
            transform.Translate(straightDirection * speed * Time.deltaTime, Space.World);
            directionInitialized = true;
        }
        else if (!directionInitialized)
        {
            // Aguarda o primeiro frame para receber a direção
            return;
        }
         else
         {
              Destroy(gameObject); // Nenhum alvo e nenhuma direção definida
         }
    }

    void OnTriggerEnter(Collider other)
{

        if (other.CompareTag("Props"))
        {
        Destroy(gameObject);
        }

    if (other.CompareTag("Enemy"))
    {
        if (projectileType == "Fire")
        {
            // Dano total no inimigo atingido diretamente
            IDamageable mainTarget = other.GetComponentInParent<IDamageable>();
            if (mainTarget != null)
            {
                mainTarget.TakeDamage(damage);
            }

            // Aplica 50% de dano em outros inimigos próximos
            ApplyAreaDamage(transform.position, other);
        }
        else
        {
            IDamageable damageable = other.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }
}

void ApplyAreaDamage(Vector3 impactPosition, Collider mainTarget)
{
    Collider[] hitColliders = Physics.OverlapSphere(impactPosition, areaRadius);
    foreach (var hitCollider in hitColliders)
    {
        if (hitCollider.CompareTag("Enemy") && hitCollider != mainTarget)
        {
            IDamageable damageable = hitCollider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage / 2f);
            }
        }
    }
}


}
