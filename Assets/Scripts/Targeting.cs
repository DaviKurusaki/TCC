using UnityEngine;

public class Targeting : MonoBehaviour
{
    public float rotationSpeed = 5f;
    public float detectionRange = 10f; // Distância para considerar o inimigo relevante
    public float meleeLookRange = 2f;  // Distância para olhar inimigo no melee

    private Transform target;
    private PlayerAttack playerAttack;
    private CharacterController controller;

    void Start()
    {
        playerAttack = GetComponent<PlayerAttack>();
        controller = GetComponent<CharacterController>(); // ou seu script de movimento
    }

    void Update()
    {
        if (playerAttack == null) return;

        FindNearestEnemy();
        RotateToTarget();
    }

    void FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < shortestDistance && distance <= detectionRange)
            {
                shortestDistance = distance;
                nearestEnemy = enemy;
            }
        }

        target = nearestEnemy != null ? nearestEnemy.transform : null;
    }

    void RotateToTarget()
    {
        if (target == null)
        {
            RotateWithMovement();
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (playerAttack.IsUsingMagic())
{
    RotateTowards(target.position); // Com magia, sempre olha para inimigo
}
else if (playerAttack.IsUsingMelee() && distanceToTarget <= meleeLookRange)
{
    RotateTowards(target.position); // Com melee, olha se estiver perto
}
else
{
    RotateWithMovement(); // Caso contrário, gira com base no movimento
}

    }

    void RotateWithMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical);

        if (moveDirection.sqrMagnitude > 0.1f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(moveDirection.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }

    void RotateTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }
}
