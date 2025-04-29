using UnityEngine;

public class IceProjectile : MonoBehaviour
{
    public float damage = 5f;
    public float freezeTime = 2f;
    public float speed = 20f;
    public float lifeTime = 3f;

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
            EnemyAI enemy = other.GetComponent<EnemyAI>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                StartCoroutine(FreezeEnemy(enemy));
            }
        }

        Destroy(gameObject);
    }

    System.Collections.IEnumerator FreezeEnemy(EnemyAI enemy)
{
    UnityEngine.AI.NavMeshAgent agent = enemy.GetComponent<UnityEngine.AI.NavMeshAgent>();
    if (agent != null)
    {
        float originalSpeed = agent.speed;
        agent.speed = originalSpeed * 0.3f; // Reduz pra 30% da velocidade
        yield return new WaitForSeconds(freezeTime);
        agent.speed = originalSpeed;
    }
}

}
