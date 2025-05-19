using UnityEngine;

public class FallingIcicle : MonoBehaviour
{
    public float fallForce = 20f;
    public int damage = 15;
      public GameObject explosionEffectPrefab;

    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(Vector3.down * fallForce, ForceMode.Impulse);
        Destroy(gameObject, 4f);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerHealth>()?.TakeDamage(damage);
        }

         if (explosionEffectPrefab != null)
        {
            GameObject explosion = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            Destroy(explosion, 3f);
        }

        Destroy(gameObject);
    }
}