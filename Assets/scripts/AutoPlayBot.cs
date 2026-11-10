using UnityEngine;

public class AutoPlayBot : MonoBehaviour
{
    [Header("Ustawienia Bota")]
    public bool isBotActive = true;
    public float sightDistance = 8f; 
    public float blindSpotDistance = 2.5f; 

    private carControler car;
    
    private float timeAlive = 0f;
    private float maxSpeedModReached = 1.0f;
    private float maxSpawnRateReached = 1.0f;

    private float targetX = 0f; 
    
    private void Start()
    {
        car = GetComponent<carControler>();
        targetX = transform.position.x;
    }

    private void Update()
    {
        if (!isBotActive || car == null) return;

        timeAlive += Time.deltaTime;
        if (FuzzyInferenceEngine.CurrentEnemySpeedMod > maxSpeedModReached)
            maxSpeedModReached = FuzzyInferenceEngine.CurrentEnemySpeedMod;
        if (FuzzyInferenceEngine.CurrentSpawnRateMod < maxSpawnRateReached)
            maxSpawnRateReached = FuzzyInferenceEngine.CurrentSpawnRateMod; 

        float leftLane = -1.5f;
        float centerLane = 0f;
        float rightLane = 1.5f;

        float checkY = transform.position.y - blindSpotDistance;
        float totalSight = sightDistance + blindSpotDistance;

        // 1. Zbieramy "wyniki" dla każdego pasa (ILE JEDNOSTEK WOLNEJ DROGI MAMY)
        float centerDist = GetLaneClearance(centerLane, checkY, totalSight);
        float leftDist = GetLaneClearance(leftLane, checkY, totalSight);
        float rightDist = GetLaneClearance(rightLane, checkY, totalSight);

        // Ustalmy, ile miejsca ma nasz AKTUALNY pas
        float currentDist = centerDist;
        if (targetX == leftLane) currentDist = leftDist;
        else if (targetX == rightLane) currentDist = rightDist;

        // 2. Próg paniki (Kiedy bot zaczyna szukać ucieczki). 
        // Skalujemy go z prędkością AI, żeby przy szybkiej grze reagował wcześniej!
        float panicThreshold = 7.5f * FuzzyInferenceEngine.CurrentEnemySpeedMod;

        // 3. Logika Decyzyjna (Ocena odległości, a nie tylko obecności!)
        if (currentDist < panicThreshold)
        {
            float bestDist = currentDist;
            float bestTarget = targetX;

            // Zabezpieczenie: Możemy zmienić tylko na pas SĄSIADUJĄCY
            if (targetX == leftLane)
            {
                if (centerDist > bestDist) { bestDist = centerDist; bestTarget = centerLane; }
            }
            else if (targetX == rightLane)
            {
                if (centerDist > bestDist) { bestDist = centerDist; bestTarget = centerLane; }
            }
            else if (targetX == centerLane)
            {
                // Ze środka możemy uciec w lewo lub w prawo. Wybieramy ten, który ma WIĘCEJ wolnego miejsca.
                if (leftDist > bestDist) { bestDist = leftDist; bestTarget = leftLane; }
                if (rightDist > bestDist) { bestDist = rightDist; bestTarget = rightLane; }
            }

            targetX = bestTarget;
        }
        else
        {
            // Opcjonalnie: Jeśli jedziemy bokiem, a środek jest całkowicie, w 100% pusty, wracamy na środek.
            if (targetX != centerLane && centerDist >= totalSight - 0.1f)
            {
                targetX = centerLane;
            }
        }

        // 4. Fizyczne skręcanie kół
        if (transform.position.x < targetX - 0.1f)
        {
            car.PressLeftUp();
            car.PressRightDown(); 
        }
        else if (transform.position.x > targetX + 0.1f)
        {
            car.PressRightUp();
            car.PressLeftDown(); 
        }
        else
        {
            car.PressLeftUp();
            car.PressRightUp();
        }
    }

    // NOWE: Metoda zwraca konkretną LICZBĘ zmiennoprzecinkową (odległość w metrach do najbliższego auta)
    private float GetLaneClearance(float laneX, float startY, float maxDistance)
    {
        Vector2 startPos = new Vector2(laneX, startY);
        RaycastHit2D[] hits = Physics2D.CircleCastAll(startPos, 0.5f, Vector2.up, maxDistance);
        
        float closestDistance = maxDistance; // Domyślnie zakładamy, że cała droga jest wolna
        
        foreach (var hit in hits)
        {
            if (hit.collider != null && hit.collider.CompareTag("EnemyCar"))
            {
                // Zapisujemy odległość tylko TEGO NAJBLIŻSZEGO wroga na danym pasie
                if (hit.distance < closestDistance)
                {
                    closestDistance = hit.distance;
                }
            }
        }
        
        return closestDistance;
    }

    private void OnDestroy()
    {
        if (isBotActive)
        {
            Debug.LogWarning($"=== RAPORT TESTOWY BOTA ===\n" +
                             $"Czas przetrwania: {timeAlive:F1} sekund\n" +
                             $"Max Prędkość Wrogów: x{maxSpeedModReached:F2}\n" +
                             $"Max Gęstość Wrogów: x{maxSpawnRateReached:F2}\n" +
                             $"AI testowe zakończyło przejazd.");
        }
    }
}