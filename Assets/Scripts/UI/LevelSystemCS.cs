using UnityEngine;

public class LevelSystem : MonoBehaviour
{
    public delegate void OnLevelUp();
    public static event OnLevelUp onLevelUp;

    public int currentXP = 0;
    public int level = 1;
    public int xpToNextLevel = 100;

    public void GainXP(int amount)
    {
        currentXP += amount;

        if (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        level++;
        currentXP -= xpToNextLevel;
        xpToNextLevel += 50;

        onLevelUp?.Invoke(); // Chama o evento de level up
    }
}
