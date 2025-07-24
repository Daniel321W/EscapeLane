using UnityEngine;

public class carSpawner : MonoBehaviour
{
    public GameObject[] cars;
    public float maxPos = 2.1f;
    public float delayTimer = 1f;

    private float _timer;

    private void Start()
    {
        _timer = delayTimer;
    }

    private void Update()
    {
        _timer -= Time.deltaTime;

        if (_timer <= 0f)
        {
            Vector3 spawnPosition = new Vector3(Random.Range(-maxPos, maxPos), transform.position.y, transform.position.z);
            int carIndex = Random.Range(0, cars.Length);
            Instantiate(cars[carIndex], spawnPosition, transform.rotation);
            _timer = delayTimer;
        }
    }
}
