using UnityEngine;
using UnityEngine.UI;

public class UpgradeUiManager : MonoBehaviour
{
    // Painéis de upgrade
    public GameObject[] upgradePanels; // Array de painéis (seleção de magia e upgrades)
    public Button fireButton;
    public Button iceButton;
    public Button lightningButton;
    public Button upgrade1Button;
    public Button upgrade2Button;
    public Button upgrade3Button;

    private PlayerAttack playerAttack;
    private PlayerHealth playerHealth;
    private LevelSystem levelSystem;

    public GameObject upgradePanel1; // Primeiro painel de upgrade
public GameObject upgradePanel2; // Segundo painel de upgrade


    void Start()
    {
        // Encontra os scripts para modificar o tipo de magia, saúde e sistema de nível
        playerAttack = FindObjectOfType<PlayerAttack>();
        playerHealth = FindObjectOfType<PlayerHealth>();
        levelSystem = FindObjectOfType<LevelSystem>();

        // Esconde todos os painéis de upgrades no início
        foreach (var panel in upgradePanels)
        {
            panel.SetActive(false);
        }

        // Atribui os eventos aos botões de seleção de magia
        fireButton.onClick.AddListener(() => SelectMagic("Fire"));
        iceButton.onClick.AddListener(() => SelectMagic("Ice"));
        lightningButton.onClick.AddListener(() => SelectMagic("Lightning"));

      /*   // Atribui os eventos aos botões de upgrades
        upgrade1Button.onClick.AddListener(() => SelectUpgrade(1)); // Aumento de dano
        upgrade2Button.onClick.AddListener(() => SelectUpgrade(2)); // Aumento de velocidade
        upgrade3Button.onClick.AddListener(() => SelectUpgrade(3)); // Aumentode vida  */

        // Escuta o evento de level up e exibe o painel correspondente
        PlayerXP.onLevelUp += ShowUpgradePanel;
    }

   /*  void OnDestroy()
    {
        // Remove a inscrição do evento corretamente
        PlayerXP.onLevelUp -= ShowUpgradePanel;
    } */

    void ShowUpgradePanel()
{
    // Verifica o nível atual do jogador e exibe o painel correto
    if (levelSystem.level == 2)
    {
        Debug.Log("Exibindo painel de seleção de magia (nível 2)");
        ShowPanel(1); // Painel de seleção de magia no nível 2
    }
    else if (levelSystem.level == 3)
    {
        Debug.Log("Exibindo painel de upgrade 1 (nível 3)");
        ShowPanel(2); // Painel de upgrade 1 no nível 3
    }
    // Continue da mesma forma para outros níveis
}


    void ShowPanel(int panelIndex)
{
    // Esconde os dois painéis antes de exibir o que está selecionado
    upgradePanel1.SetActive(false);
    upgradePanel2.SetActive(false);

    // Exibe o painel correto com base no índice
    if (panelIndex == 1)
    {
        upgradePanel1.SetActive(true);
    }
    else if (panelIndex == 2)
    {
        upgradePanel2.SetActive(true);
    }
    else
    {
        Debug.LogError("Índice de painel inválido: " + panelIndex);
    }
}


    void SelectMagic(string type)
    {
        // Altera o tipo de magia com base na escolha do jogador
        switch (type)
        {
            case "Fire":
                playerAttack.SetMageType(PlayerAttack.MageType.Fire);
                break;
            case "Ice":
                playerAttack.SetMageType(PlayerAttack.MageType.Ice);
                break;
            case "Lightning":
                playerAttack.SetMageType(PlayerAttack.MageType.Lightning);
                break;
        }

        // Exibe o painel de upgrade 1 (nível 3) após a seleção de magia
        ShowPanel(1);
    }

   /*  void SelectUpgrade(int upgradeType)
    {
        // Lógica para os upgrades
        switch (upgradeType)
        {
            case 1:
                // Aumenta o dano do projétil
                playerAttack.UpgradeDamage(5f);  // Aumento de dano
                break;
            case 2:
                // Aumenta a velocidade do projétil
                playerAttack.UpgradeSpeed(2f);  // Aumento da velocidade
                break;
            case 3:
                // Aumenta a vida do player
                playerHealth.IncreaseMaxHealth(50f);  // Aumento da vida
                break;
        }

        // Oculta o painel de upgrades e retoma o jogo
        foreach (var panel in upgradePanels)
        {
            panel.SetActive(false);
        }
        Time.timeScale = 1f;
    } */
}
