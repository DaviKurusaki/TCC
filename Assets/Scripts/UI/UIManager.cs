using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Slider healthBar;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI waveText;

    public int score = 0;
    public int wave = 1;

    public void UpdateHealth(float current, float max)
    {
        healthBar.value = current;
    }

    public void AddScore(int amount)
    {
        score += amount;
        scoreText.text = score.ToString() + " Kills";

    }

    public void SetWave(int waveNumber)
    {
        wave = waveNumber;
        waveText.text = "Wave " + wave;
    }

public void NextWave()
{
    wave++;
    waveText.text = "Wave " + wave;
}



}
