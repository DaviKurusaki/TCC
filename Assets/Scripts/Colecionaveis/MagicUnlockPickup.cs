using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MagicUnlockPickup : MonoBehaviour
{
    public PlayerAttack.MageType magicType; // O tipo da magia que será desbloqueada
    public float floatAmplitude = 0.5f;     // Amplitude para o movimento de flutuação
    public float floatFrequency = 1f;       // Frequência para o movimento de flutuação
    private Vector3 startPos;               // Posição inicial do pickup

    void Start()
    {
        startPos = transform.position;
        var col = GetComponent<Collider>();
        col.isTrigger = true; // Certifica-se de que o collider é um trigger
    }

    void Update()
    {
        // Movimento de flutuação para cima e para baixo
        float y = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = startPos + Vector3.up * y;
    }

    void OnTriggerEnter(Collider other)
    {
        // Verifica se o objeto que entrou no trigger tem o PlayerAmmoManager
        var mgr = other.GetComponent<PlayerAmmoManager>();
        if (mgr != null)
        {
            // Converte o tipo de magia (PlayerAttack.MageType) para AmmoType e desbloqueia
            mgr.UnlockAmmo((PlayerAmmoManager.AmmoType)magicType);
            Destroy(gameObject); // Destrói o pickup após ser coletado
        }
    }
}
