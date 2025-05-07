using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public GameObject painelControles;

    public void Jogar()
    {
        SceneManager.LoadScene("SampleScene"); //Primeira cena
    }

    public void AbrirControles()
    {
        painelControles.SetActive(true);
    }

    public void FecharControles()
    {
        painelControles.SetActive(false);
    }

    public void Sair()
    {
        Application.Quit();
        Debug.Log("Saindo do jogo...");
    }
}
