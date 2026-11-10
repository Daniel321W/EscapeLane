using UnityEngine;

public class FuzzyInferenceEngine : MonoBehaviour
{
    public static FuzzyInferenceEngine Instance { get; private set; }

    [Header("Ustawienia AI")]
    public float highSkillThreshold = 0.6f; 
    public float lowSkillThreshold = 0.3f;

    // Aktualne mnożniki (płynnie zmieniające się)
    public static float CurrentSpawnRateMod = 1.0f;
    public static float CurrentEnemySpeedMod = 1.0f;
    public static float CurrentLaneChangeChance = 0f; 
    public static float CurrentPowerUpSpawnMod = 1.0f;
    public static int sessionDeathCount = 0;

    // Docelowe mnożniki (wyliczone przez AI co 1 sek.)
    private float targetSpawnRateMod = 1.0f;
    private float targetEnemySpeedMod = 1.0f;
    private float targetLaneChangeChance = 0f;
    private float targetPowerUpSpawnMod = 1.0f;

    private float survivalTime = 0f;
    private float totalNearMissMultiplier = 0f;
    private int nearMissCount = 0;
    private int collectedPowerUps = 0;
    
    private float calcTimer = 1f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

   private bool forgivenThisRun = false;

    private void Start()
    {
        // 1. Ładujemy historię zgonów z poprzednich wyścigów
        sessionDeathCount = PlayerPrefs.GetInt("RecentDeaths", 0); 
        forgivenThisRun = false;
        
        CurrentSpawnRateMod = 1.0f;
        CurrentEnemySpeedMod = 1.0f;
        CurrentLaneChangeChance = 0f;
        CurrentPowerUpSpawnMod = 1.0f;
        targetSpawnRateMod = 1.0f;
        targetEnemySpeedMod = 1.0f;
        targetLaneChangeChance = 0f;
        targetPowerUpSpawnMod = 1.0f;

        survivalTime = 0f;
        totalNearMissMultiplier = 0f;
        nearMissCount = 0;
        collectedPowerUps = 0;
    }

    private void Update()
    {
        survivalTime += Time.deltaTime;
        calcTimer -= Time.deltaTime;

        // --- SYSTEM WYBACZANIA BŁĘDÓW ---
        // Jeśli przetrwałeś 35 sekund, system usuwa jeden zgon z pamięci
        if (survivalTime > 35f && sessionDeathCount > 0 && !forgivenThisRun)
        {
            sessionDeathCount--;
            PlayerPrefs.SetInt("RecentDeaths", sessionDeathCount);
            PlayerPrefs.Save();
            forgivenThisRun = true; // Żeby wybaczył tylko raz na wyścig
            Debug.Log("[Fuzzy AI] Gracz gra dobrze! Wybaczono 1 stary zgon.");
        }

        if (calcTimer <= 0f)
        {
            float avgMiss = nearMissCount > 0 ? (totalNearMissMultiplier / nearMissCount) : 0f;
            CalculateDDA(survivalTime, avgMiss, sessionDeathCount);
            calcTimer = 1f; 
        }

        float lerpSpeed = Time.deltaTime * 1.5f; 
        CurrentSpawnRateMod = Mathf.Lerp(CurrentSpawnRateMod, targetSpawnRateMod, lerpSpeed);
        CurrentEnemySpeedMod = Mathf.Lerp(CurrentEnemySpeedMod, targetEnemySpeedMod, lerpSpeed);
        CurrentLaneChangeChance = Mathf.Lerp(CurrentLaneChangeChance, targetLaneChangeChance, lerpSpeed);
        CurrentPowerUpSpawnMod = Mathf.Lerp(CurrentPowerUpSpawnMod, targetPowerUpSpawnMod, lerpSpeed);
    }

    public void RegisterNearMiss(float multiplier)
    {
        nearMissCount++;
        totalNearMissMultiplier += multiplier;
    }
    public void RegisterPowerUp()
    {
        collectedPowerUps++;
    }

    public float CalculateDDA(float survivalTime, float avgNearMissMultiplier, int deathCount)
    {
        float normalizedSurvival = Mathf.Clamp01(survivalTime / 60f); 
        float normalizedEffectiveness = Mathf.Clamp01(avgNearMissMultiplier / 2.0f);
        
        // Zmniejszyłem karę za zgon z 0.1 na 0.05, żeby system wybaczał pojedyncze błędy
        float deathPenalty = Mathf.Clamp01(deathCount * 0.05f); 

        float powerUpPenalty = Mathf.Clamp01(collectedPowerUps * 0.02f);

        float overallSkillScore = ((normalizedSurvival + normalizedEffectiveness) / 2f) - deathPenalty;
        overallSkillScore = Mathf.Clamp01(overallSkillScore);
        
        if (overallSkillScore >= highSkillThreshold)
        {
            // BARDZO DOBRZE IDZIE
            targetSpawnRateMod = 0.5f; 
            targetEnemySpeedMod = 1.4f; 
            targetLaneChangeChance = 0.4f; 
            targetPowerUpSpawnMod = 1.8f;
        }
        else if (overallSkillScore <= lowSkillThreshold)
        {
             // SŁABO IDZIE
             targetSpawnRateMod = 1.5f; 
             targetEnemySpeedMod = 0.8f;
             targetLaneChangeChance = 0.0f;
             targetPowerUpSpawnMod = 0.6f;
        }
        else
        {
             // ŚREDNIO
             targetSpawnRateMod = 1.0f;
             targetEnemySpeedMod = 1.0f;
             targetLaneChangeChance = 0.15f;
             targetPowerUpSpawnMod = 1.0f;
        }

        return overallSkillScore;
    }
}