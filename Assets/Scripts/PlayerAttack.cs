using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public enum MageType { Basic, Fire, Ice, Lightning }
    public MageType currentMage = MageType.Basic;

    public GameObject basicProjectile;
    public GameObject fireProjectile;
    public GameObject iceProjectile;
    public GameObject lightningProjectile;

    public Transform firePoint;  // Posição de disparo do projétil
    public float missileSpeed = 10f;
    public float detectionRadius = 15f;

    public float projectileDamage = 5f;  // Dano do projétil
    public float projectileSpeed = 10f;  // Velocidade do projétil

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            // Encontra o inimigo mais próximo
            Transform nearestEnemy = GetNearestEnemyInRange();

            // Obtém o projétil com base no tipo de magia atual
            GameObject projectileToShoot = GetProjectileByType();

            // Instancia o projétil na posição do firePoint e com a rotação do player
            GameObject projectile = Instantiate(projectileToShoot, firePoint.position, Quaternion.LookRotation(firePoint.forward));

            // Verifica se o projétil tem o componente HomingProjectile
            HomingProjectile homingProjectile = projectile.GetComponent<HomingProjectile>();

            // Se o projétil for homing e o inimigo estiver presente
            if (homingProjectile != null && nearestEnemy != null)
            {
                homingProjectile.SetTarget(nearestEnemy, currentMage.ToString());

                // Ajusta o dano e a velocidade do projétil com base nos upgrades
                homingProjectile.SetDamage(projectileDamage);
                homingProjectile.SetSpeed(projectileSpeed);
            }
        }
    }

    GameObject GetProjectileByType()
    {
        // Retorna o projétil de acordo com o tipo de magia
        switch (currentMage)
        {
            case MageType.Fire:
                return fireProjectile;
            case MageType.Ice:
                return iceProjectile;
            case MageType.Lightning:
                return lightningProjectile;
            default:
                return basicProjectile; // Magia básica
        }
    }

    Transform GetNearestEnemyInRange()
    {
        // Encontra o inimigo mais próximo no alcance de detecção
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Transform closest = null;
        float shortestDistance = detectionRadius;

        foreach (GameObject enemy in enemies)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist <= shortestDistance)
            {
                shortestDistance = dist;
                closest = enemy.transform;
            }
        }

        return closest;
    }

    public void SetMageType(MageType newType)
    {
        // Altera o tipo de magia
        currentMage = newType;
    }

    // Métodos para aplicar os upgrades
    public void UpgradeDamage(float addedDamage)
    {
        projectileDamage += addedDamage;  // Aumenta o dano do projétil
        Debug.Log("Dano do projétil aumentado para: " + projectileDamage);
    }

    public void UpgradeSpeed(float addedSpeed)
    {
        missileSpeed += addedSpeed;  // Aumenta a velocidade do projétil
        Debug.Log("Velocidade do projétil aumentada para: " + missileSpeed);
    }
}
