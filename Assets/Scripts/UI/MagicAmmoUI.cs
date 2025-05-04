using UnityEngine;
using UnityEngine.UI;

public class MagicAmmoUI : MonoBehaviour
{
    [Header("Ammo Display")]
    public Image[] ammoIcons; // ícones para exibir munição da magia

    [Header("Mage Selection UI")]
    public Image[] mageIcons;      // Imagens na UI (molduras)
    public Sprite[] mageSprites;   // Sprites de magias desbloqueadas (ex: fogo, gelo...)
    public Sprite lockedSprite;    // Sprite para magia bloqueada (ex: cadeado)


    public void UpdateAmmoDisplay(int currentAmmo, int maxAmmo)
    {
        for (int i = 0; i < ammoIcons.Length; i++)
        {
            ammoIcons[i].enabled = i < currentAmmo;
        }
    }

    /// <summary>
    /// Atualiza a UI de seleção de magias
    /// </summary>
    /// <param name="current">Índice da magia atual selecionada</param>
    /// <param name="unlocked">Array de quais magias estão desbloqueadas</param>
    public void SetMage(PlayerAttack.MageType current, bool[] unlocked)
    {
        for (int i = 0; i < mageIcons.Length; i++)
        {
            if (unlocked[i])
            {
                mageIcons[i].sprite = mageSprites[i];
                mageIcons[i].color = (i == (int)current) ? Color.white : new Color(1f, 1f, 1f, 0.5f);
            }
            else
            {
                mageIcons[i].sprite = lockedSprite;
                mageIcons[i].color = new Color(1f, 1f, 1f, 0.5f);
            }
        }
    }
}
