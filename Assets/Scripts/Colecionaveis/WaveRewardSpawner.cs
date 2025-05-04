using UnityEngine;

public class WaveRewardSpawner : MonoBehaviour
{
    [System.Serializable]
    public class Reward
    {
        public int waveNumber; // Ex: 1, 2, 3
        public GameObject rewardPrefab; // Prefab da munição
        public Transform spawnPoint; // Onde vai aparecer
    }

    public Reward[] rewards;

    public void SpawnRewardForWave(int wave)
    {
        foreach (var reward in rewards)
        {
            if (reward.waveNumber == wave)
            {
                Instantiate(reward.rewardPrefab, reward.spawnPoint.position, Quaternion.identity);
        
            }
        }
    }
}
