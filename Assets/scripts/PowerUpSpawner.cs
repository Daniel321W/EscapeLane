using System.Collections;
using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    public GameObject[] powerUpPrefabs;     // Tablica prefabów power-upów
    public float xMin = -2.1f;
    public float xMax = 2.1f;
    public float spawnY = 6f;

    public float minSpawnTime = 5f;       // Minimalny czas między power-upami
    public float maxSpawnTime = 15f;      // Maksymalny czas między power-upami

    private void Start()
    {
        StartCoroutine(SpawnPowerUpRandomly());
    }

    IEnumerator SpawnPowerUpRandomly()
    {
        while (true)
        {
            // ZABEZPIECZENIE przed pustą tablicą
            if (powerUpPrefabs == null || powerUpPrefabs.Length == 0)
            {
                yield return new WaitForSeconds(1f);
                continue;
            }

            // NOWE: Pobieramy modyfikator z systemu SI.
            float aiModifier = 1.0f;
            if (FuzzyInferenceEngine.Instance != null)
            {
                aiModifier = FuzzyInferenceEngine.CurrentPowerUpSpawnMod;
            }

            // Czas oczekiwania jest teraz mnożony przez decyzję SI!
            float waitTime = Random.Range(minSpawnTime, maxSpawnTime) * aiModifier;
            yield return new WaitForSeconds(waitTime);

            int randomIndex = Random.Range(0, powerUpPrefabs.Length);
            GameObject prefabToSpawn = powerUpPrefabs[randomIndex];

            if (prefabToSpawn != null)
            {
                float randomX = Random.Range(xMin, xMax);
                Vector3 spawnPosition = new Vector3(randomX, spawnY, 0f);
                Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
            }
        }
    }
}
