using UnityEngine;
using System.Collections.Generic;

public class carSpawner : MonoBehaviour
{
    public GameObject[] carPrefabs; 
    public float maxPos = 2.1f;
    public float delayTimer = 1f;
    public int poolSize = 20; 

    private float _timer;
    private List<GameObject> pooledCars;

    private void Start()
    {
        _timer = delayTimer;
        pooledCars = new List<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            int prefabIndex = i % carPrefabs.Length;
            CreateCarForPool(prefabIndex);
        }
    }

    private GameObject CreateCarForPool(int prefabIndex)
    {
        GameObject obj = Instantiate(carPrefabs[prefabIndex]);
        obj.SetActive(false);
        pooledCars.Add(obj);
        return obj;
    }

    private GameObject GetPooledCar()
    {
       
        List<GameObject> availableCars = new List<GameObject>();

        for (int i = 0; i < pooledCars.Count; i++)
        {
            if (!pooledCars[i].activeInHierarchy)
            {
                availableCars.Add(pooledCars[i]);
            }
        }

        if (availableCars.Count > 0)
        {
            
            int randomIndex = Random.Range(0, availableCars.Count);
            return availableCars[randomIndex];
        }
        
       
        int randomNewIndex = Random.Range(0, carPrefabs.Length);
        return CreateCarForPool(randomNewIndex);
    }

    private void Update()
    {
        _timer -= Time.deltaTime;

        if (_timer <= 0f)
        {
            SpawnCar();
            _timer = delayTimer;
        }
    }

    private void SpawnCar()
    {
        GameObject car = GetPooledCar();
        
        Vector3 spawnPosition = new Vector3(Random.Range(-maxPos, maxPos), transform.position.y, transform.position.z);
        car.transform.position = spawnPosition;
        car.transform.rotation = transform.rotation;
        
        car.SetActive(true);
    }
}