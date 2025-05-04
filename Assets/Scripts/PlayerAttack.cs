using UnityEngine;

public class PlayerAttack : MonoBehaviour
{

    [Header("UI Config")]
    [SerializeField] private MagicAmmoUI magicAmmoUI;

    // ------------------------------
    // Código da distância (Pistola)
    // ------------------------------
    [Header("Pistol")]
    public GameObject pistolObject;  // Objeto da varinha
    public enum MageType { Basic, Fire, Ice, Lightning }
    public MageType currentMage = MageType.Basic;

    [Header("Bullet Type")]

    public GameObject basicProjectile;
    public GameObject fireProjectile;
    public GameObject iceProjectile;
    public GameObject lightningProjectile;

    [Header("Fire Point Aim")]
    public Transform firePoint;

    [Header("Projectile Settings")]
    public float missileSpeed = 10f;
    public float detectionRadius = 15f;
    public float projectileDamage = 5f;
    public float projectileSpeed = 10f;

    [Header("Magic Ammo System")]
    public int maxMagicAmmo = 6;
    public int currentMagicAmmo;
    public float ammoRegenInterval = 1f;
    private float ammoRegenTimer;


    // ------------------------------
    // Código para a espada (Melee)
    // ------------------------------
    [Header("Melee")]

    public GameObject swordObject; // Objeto da espada

    public enum WeaponType { Magic, Melee }
    public WeaponType currentWeapon = WeaponType.Magic;

    public float meleeRange = 2f;     // Alcance do ataque com espada
    public float meleeDamage = 10f;   // Dano da espada

    // ------------------------------
    // Código para armas guardadas
    // ------------------------------
    [Header("Sheated Weapons Objects")]
    public GameObject BackSwordObject; // Objeto da espada nas costas
    public GameObject BackPistolObject; // Objeto da espada nas costas

    public MagicAmmoUI MagicAmmoUI { get => magicAmmoUI; set => magicAmmoUI = value; }

    void Start()
    {
        currentMagicAmmo = maxMagicAmmo;
        ammoRegenTimer = 0f;
        UpdateAmmoUI();
    }

    void Update()
    {

        // Regen de mana/munição mágica
        if (currentWeapon == WeaponType.Magic && currentMagicAmmo < maxMagicAmmo)
        {
            ammoRegenTimer += Time.deltaTime;
            if (ammoRegenTimer >= ammoRegenInterval)
            {
                currentMagicAmmo++;
                ammoRegenTimer = 0f;
                UpdateAmmoUI(); // atualiza no canvas
            }
        }




        // Troca de arma (botão direito do mouse)
        if (Input.GetMouseButtonDown(1))
        {
            ToggleWeapon();
        }

        // Ataque (botão esquerdo do mouse)
        if (Input.GetButtonDown("Fire1"))
        {
            if (currentWeapon == WeaponType.Magic)
            {
                ShootMagic();
            }
            else if (currentWeapon == WeaponType.Melee)
            {
                PerformMeleeAttack();
            }
        }
    }

    void ToggleWeapon()
    {
        if (currentWeapon == WeaponType.Magic)
        {
            currentWeapon = WeaponType.Melee;
            pistolObject.SetActive(false);
            swordObject.SetActive(true);
            BackPistolObject.SetActive(true);
            BackSwordObject.SetActive(false);
        }
        else
        {
            currentWeapon = WeaponType.Magic;
            pistolObject.SetActive(true);
            swordObject.SetActive(false);
            BackPistolObject.SetActive(false);
            BackSwordObject.SetActive(true);
        }
    }

    void ShootMagic()
    {

       if (currentMagicAmmo <= 0)
        return;

    currentMagicAmmo--;
    UpdateAmmoUI();

    Transform nearestEnemy = GetNearestEnemyInRange();
    GameObject projectileToShoot = GetProjectileByType();

    GameObject projectile = Instantiate(projectileToShoot, firePoint.position, Quaternion.LookRotation(firePoint.forward));
    HomingProjectile homingProjectile = projectile.GetComponent<HomingProjectile>();

    if (homingProjectile != null)
    {
        homingProjectile.SetDamage(projectileDamage);
        homingProjectile.SetSpeed(projectileSpeed);

        if (nearestEnemy != null)
        {
            homingProjectile.SetTarget(nearestEnemy, currentMage.ToString());
        }
        else
        {
            // Caso não tenha alvo, projétil segue em linha reta
            homingProjectile.SetTarget(null, currentMage.ToString());
             homingProjectile.SetStraightDirection(firePoint.forward);
        }
    }
    }

   void PerformMeleeAttack()
{
    // Faz o ataque em área com base no alcance da espada (meleeRange)
    Collider[] hitEnemies = Physics.OverlapSphere(transform.position + transform.forward, meleeRange);

    foreach (Collider enemy in hitEnemies)
    {
        if (enemy.CompareTag("Enemy"))
        {
            IDamageable damageable = enemy.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(meleeDamage);
            }
        }
    }
}


    GameObject GetProjectileByType()
    {
        switch (currentMage)
        {
            case MageType.Fire:
                return fireProjectile;
            case MageType.Ice:
                return iceProjectile;
            case MageType.Lightning:
                return lightningProjectile;
            default:
                return basicProjectile;
        }
    }

    Transform GetNearestEnemyInRange()
    {
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
        currentMage = newType;
         UpdateAmmoUI();
    }

    // Gizmo para visualizar o raio da espada no editor
    void OnDrawGizmosSelected()
    {
        if (currentWeapon == WeaponType.Melee)
        {
            Gizmos.color = Color.red;
            Vector3 attackOrigin = transform.position + transform.forward * 1f;
            Gizmos.DrawWireSphere(attackOrigin, meleeRange);
        }
    }

    public bool IsUsingMagic()
    {
        return currentWeapon == WeaponType.Magic;
    }

    public bool IsUsingMelee()
    {
        return currentWeapon == WeaponType.Melee;
    }

   public void UpdateAmmoUI()
    {
        if (MagicAmmoUI != null)
        {
            MagicAmmoUI.UpdateAmmoDisplay(currentMagicAmmo, maxMagicAmmo);
        }
    }


}


