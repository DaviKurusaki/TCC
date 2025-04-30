using UnityEngine;
using UnityEngine.UI;

public class MagicAmmoUI : MonoBehaviour
{
    public Image[] ammoIcons;

    public void UpdateAmmoDisplay(int currentAmmo, int maxAmmo)
    {
        for (int i = 0; i < ammoIcons.Length; i++)
        {
            ammoIcons[i].enabled = i < currentAmmo;
        }
    }
}
