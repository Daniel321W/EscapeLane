using System.Collections;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    public enum PowerUpType
    {
        Shield,
        Attack,
        DoublePoints 
    }

    public GameObject shieldVisual;
    public AudioClip powerUpSound;
    public AudioSource audioSource;
    public trackMove track;
    public float powerUpDuration = 5f;

    public uiManager ui;  // <- przypnij z Unity do UI managera
private float originalScoreRate = 1f;


    private carControler carController;
    private float originalCarSpeed;
    private float originalTrackSpeed;
    private bool isImmortal = false;

  private void Start()
{
    carController = GetComponent<carControler>();
    originalCarSpeed = carController.carSpeed;

    if (track == null)
        track = FindObjectOfType<trackMove>();

    if (track != null)
        originalTrackSpeed = track.speed;

    if (ui == null)
        ui = FindObjectOfType<uiManager>();
}


    public void ActivatePowerUp(PowerUpType type)
    {
        StartCoroutine(PowerUpRoutine(type));
    }

    private IEnumerator PowerUpRoutine(PowerUpType type)
    {
        if (type == PowerUpType.Shield)
        {
            isImmortal = true;
            if (shieldVisual != null)
                shieldVisual.SetActive(true);
        }

        if (audioSource != null && powerUpSound != null)
            audioSource.PlayOneShot(powerUpSound);

        carController.carSpeed *= 1.5f;
        if (track != null)
            track.speed *= 1.5f;

        EnemyCarMove[] enemies = FindObjectsOfType<EnemyCarMove>();

        if (type == PowerUpType.Attack)
        {
            foreach (EnemyCarMove enemy in enemies)
                enemy.SetEscapeMode(true);
        }

        if (type == PowerUpType.DoublePoints && ui != null)
{
    originalScoreRate = ui.scoreMultiplier;
    ui.scoreMultiplier = originalScoreRate * 4f;
}

        yield return new WaitForSeconds(powerUpDuration);

        if (type == PowerUpType.DoublePoints && ui != null)
{
    ui.scoreMultiplier = originalScoreRate;
}


        if (type == PowerUpType.Shield)
        {
            isImmortal = false;
            if (shieldVisual != null)
                shieldVisual.SetActive(false);
        }

        carController.carSpeed = originalCarSpeed;
        if (track != null)
            track.speed = originalTrackSpeed;

        if (type == PowerUpType.Attack)
        {
            foreach (EnemyCarMove enemy in enemies)
                enemy.SetEscapeMode(false);
        }
    }

    public bool IsImmortal()
    {
        return isImmortal;
    }
}
