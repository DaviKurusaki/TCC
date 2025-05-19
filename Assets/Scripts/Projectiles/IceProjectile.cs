using UnityEngine;

public class IceProjectile : MonoBehaviour
{
    public float damage = 5f;
    public float freezeTime = 2f;
    public float speed = 20f;
    public float lifeTime = 3f;
    public float freezeChance = 0.3f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyAI enemyAI = other.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                ApplyEffectToEnemyAI(enemyAI);
            }
            else
            {
                EnemyAIMelee enemyAIMelee = other.GetComponent<EnemyAIMelee>();
                if (enemyAIMelee != null)
                {
                    ApplyEffectToEnemyAIMelee(enemyAIMelee);
                }
            }
        }

        Destroy(gameObject);
    }

    void ApplyEffectToEnemyAI(EnemyAI enemy)
    {
        bool isFrozen = Random.value <= freezeChance;

        if (isFrozen)
        {
            Debug.Log("CONGELADO! Dano dobrado!");
            enemy.Freeze(freezeTime);
            enemy.TakeDamage(damage * 2f);
        }
        else
        {
            Debug.Log("Não congelou! Apenas lentidão.");
            enemy.Slow(freezeTime);
            enemy.TakeDamage(damage);
        }
    }

    void ApplyEffectToEnemyAIMelee(EnemyAIMelee enemy)
    {
        bool isFrozen = Random.value <= freezeChance;

        if (isFrozen)
        {
            Debug.Log("CONGELADO! Dano dobrado!");
            enemy.Freeze(freezeTime);
            enemy.TakeDamage(damage * 2f);
        }
        else
        {
            Debug.Log("Não congelou! Apenas lentidão.");
            enemy.Slow(freezeTime);
            enemy.TakeDamage(damage);
        }
    }
}
