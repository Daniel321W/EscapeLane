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
            float waitTime = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(waitTime);

            if (powerUpPrefabs.Length == 0)
            {
                Debug.LogWarning("Brak power-upów do spawnowania!");
                yield break; // Przerywa coroutine jeśli brak prefabów
            }

            int randomIndex = Random.Range(0, powerUpPrefabs.Length);
            GameObject prefabToSpawn = powerUpPrefabs[randomIndex];

            if (prefabToSpawn == null)
            {
                Debug.LogWarning("Prefab power-upa jest null na indeksie: " + randomIndex);
                continue; // Pomija ten spawn i czeka do następnego
            }

            float randomX = Random.Range(xMin, xMax);
            Vector3 spawnPosition = new Vector3(randomX, spawnY, 0f);

            Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        }
    }
}
