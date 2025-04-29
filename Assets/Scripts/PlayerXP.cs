using UnityEngine;

public class PlayerXP : MonoBehaviour
{
    [Header("Configurações de XP")]
    [SerializeField] private int currentXP = 0;
    [SerializeField] private int level = 1;
    [SerializeField] private int xpToNextLevel = 100;

    private UIManager ui;

    // Evento para notificar quando o jogador subir de nível
    public delegate void OnLevelUp();
    public static event OnLevelUp onLevelUp;

    void Start()
    {
        ui = FindObjectOfType<UIManager>();
    }

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

        // Notificar que o jogador subiu de nível
        onLevelUp?.Invoke(); // Chama o evento e ativa o painel de upgrades

        // Atualiza UI, se necessário (por exemplo, mostrando o XP atual)

    }
}
