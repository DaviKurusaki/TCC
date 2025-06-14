using System;
using UnityEngine;

public class PlayerAmmoManager : MonoBehaviour
{
    [Header("Referências de Componentes")]
    [Tooltip("Script de ataque do jogador")]
    public PlayerAttack playerAttack;
    [Tooltip("UI que mostra seleção de magia e munição")]
    public MagicAmmoUI magicUI;

    [Header("Magias desbloqueadas (Basic, Fire, Ice, Lightning)")]
    public bool[] unlockedMagias = new bool[4];

    private int currentIndex = 0;
    private PlayerAttack.MageType[] mageTypes;

    // Enum para representar os tipos de magia (ammo)
    public enum AmmoType
    {
        Basic,
        Fire,
        Ice,
        Lightning
    }

    void Start()
    {
        // Inicializa lista de tipos de magia a partir do enum
        mageTypes = (PlayerAttack.MageType[])System.Enum.GetValues(typeof(PlayerAttack.MageType));

        // Garante que pelo menos o Basic esteja desbloqueado
        unlockedMagias[(int)PlayerAttack.MageType.Basic] = true;

        // Define magia inicial (Basic)
        SetMageTypeByIndex(currentIndex);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Próxima magia desbloqueada
            AdvanceIndex(1);
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            // Magia anterior desbloqueada
            AdvanceIndex(-1);
        }
    }

    private void AdvanceIndex(int delta)
    {
        int start = currentIndex;
        int len = mageTypes.Length;

        do
        {
            currentIndex = (currentIndex + delta + len) % len;
        }
        while (!unlockedMagias[currentIndex] && currentIndex != start);

        SetMageTypeByIndex(currentIndex);
    }

    private void SetMageTypeByIndex(int index)
    {
        var chosen = mageTypes[index];

        // Atualiza no PlayerAttack
        if (playerAttack != null)
            playerAttack.SetMageType(chosen);

        // Atualiza na UI
        if (magicUI != null)
            magicUI.SetMage(chosen, unlockedMagias);
    }

    // Método para desbloquear magias (ammo)
    internal void UnlockAmmo(AmmoType ammoType)
    {
        // Marca o tipo de magia como desbloqueado
        unlockedMagias[(int)ammoType] = true;

        // Atualiza a UI após desbloquear a magia
        magicUI.SetMage(mageTypes[currentIndex], unlockedMagias);

        // Aqui pode ser adicionado um efeito sonoro ou visual, se necessário
        Debug.Log($"Magia {ammoType} desbloqueada!");
    }
}
