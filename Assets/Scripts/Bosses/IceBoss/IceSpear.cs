using UnityEngine;

public class IceSpear : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 5f;
    public int damage = 10;

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
        if (other.CompareTag("Player"))
        {
            // Aplique dano
            other.GetComponent<PlayerHealth>()?.TakeDamage(damage);

            // Aplique lentidão se desejar
           // other.GetComponent<PlayerMovement>()?.ApplySlow(2f, 2f); // exemplo: 2s de lentidão

            Destroy(gameObject);
        }
    }
}
