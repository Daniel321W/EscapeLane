using UnityEngine;

public class trackMove : MonoBehaviour
{
    // Ta zmienna nadal może być modyfikowana przez PowerUpManager (np. x1.5 przy przyspieszeniu)
    public float speed; 

    // Akumulator przesunięcia (żeby zapobiec "skakaniu" tekstury przy zmianie prędkości)
    private float currentOffset = 0f;
    private Renderer rend;

    private void Start()
    {
        rend = GetComponent<Renderer>();
    }

    private void Update()
    {
        // 1. Obliczamy finalną prędkość całego świata (bazowa prędkość * modyfikator AI)
        float finalWorldSpeed = speed * FuzzyInferenceEngine.CurrentEnemySpeedMod;
        
        // 2. Dodajemy przesunięcie powoli co klatkę (Time.deltaTime)
        currentOffset += finalWorldSpeed * Time.deltaTime;
        
        // 3. Aplikujemy przesunięcie do tekstury drogi
        rend.material.mainTextureOffset = new Vector2(0f, currentOffset);
    }
}