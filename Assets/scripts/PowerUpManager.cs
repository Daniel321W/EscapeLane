using System.Collections;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    public GameObject shieldVisual;      // Normalny GameObject z grafiką tarczy (sprite, animacja, itp.)
    public AudioClip powerUpSound;       // Normalny plik audio (mp3/wav)
    public AudioSource audioSource;      // AudioSource z Unity (może być przypięty do gracza)
    public trackMove track;              // Skrypt od tła
    public float powerUpDuration = 5f;

    private bool isImmortal = false;
    private float originalCarSpeed;
    private float originalTrackSpeed;
    private float enemySpeedMultiplier = 1.5f;

    private carControler carController;

    private void Start()
    {
        carController = GetComponent<carControler>();
        originalCarSpeed = carController.carSpeed;

        if (track == null)
            track = FindObjectOfType<trackMove>(); // <- SZUKA TRACK NA SCENIE

        if (track != null)
            originalTrackSpeed = track.speed;
    }

    public void ActivatePowerUp()
    {
        StartCoroutine(PowerUpRoutine());
    }

    private IEnumerator PowerUpRoutine()
    {
        isImmortal = true;

        if (shieldVisual != null)
            shieldVisual.SetActive(true);

        if (audioSource != null && powerUpSound != null)
            audioSource.PlayOneShot(powerUpSound);

        // Boost prędkości
        carController.carSpeed *= 1.5f;

        if (track != null)
            track.speed *= 1.5f;

        // Zwiększ prędkość wszystkich istniejących przeciwników
        EnemyCarMove[] enemies = FindObjectsOfType<EnemyCarMove>();
        foreach (EnemyCarMove enemy in enemies)
            enemy.speed *= enemySpeedMultiplier;

        yield return new WaitForSeconds(powerUpDuration);

        isImmortal = false;

        if (shieldVisual != null)
            shieldVisual.SetActive(false);

        carController.carSpeed = originalCarSpeed;

        if (track != null)
            track.speed = originalTrackSpeed;

        foreach (EnemyCarMove enemy in enemies)
            enemy.speed /= enemySpeedMultiplier;
    }

    public bool IsImmortal()
    {
        return isImmortal;
    }
}
