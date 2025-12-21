using System.Collections;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    public enum PowerUpType { Shield, Attack, DoublePoints }

    public GameObject shieldVisual;
    public AudioClip powerUpSound;
    public AudioSource audioSource;
    public trackMove track;
    public float powerUpDuration = 5f;
    public uiManager ui;

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
    {
        
        track = FindAnyObjectByType<trackMove>();
    }

    if (track != null)
        originalTrackSpeed = track.speed;

    if (ui == null)
    {
        
        ui = FindAnyObjectByType<uiManager>();
    }
}

    public void ActivatePowerUp(PowerUpType type)
    {
        StartCoroutine(PowerUpRoutine(type));
    }

    private IEnumerator PowerUpRoutine(PowerUpType type)
    {
        
        if (audioSource != null && powerUpSound != null)
            audioSource.PlayOneShot(powerUpSound);

        
        carController.carSpeed *= 1.5f;
        if (track != null) track.speed *= 1.5f;

        
        if (type == PowerUpType.Shield)
        {
            isImmortal = true;
            if (shieldVisual != null) shieldVisual.SetActive(true);
        }
        else if (type == PowerUpType.Attack)
        {
            
            if (EnemyCarMove.OnEscapeModeChanged != null)
                EnemyCarMove.OnEscapeModeChanged(true);
        }
        else if (type == PowerUpType.DoublePoints && ui != null)
        {
            originalScoreRate = ui.scoreMultiplier;
            ui.scoreMultiplier = originalScoreRate * 4f;
        }

        
        yield return new WaitForSeconds(powerUpDuration);

        
        if (type == PowerUpType.Shield)
        {
            isImmortal = false;
            if (shieldVisual != null) shieldVisual.SetActive(false);
        }
        else if (type == PowerUpType.Attack)
        {
            
            if (EnemyCarMove.OnEscapeModeChanged != null)
                EnemyCarMove.OnEscapeModeChanged(false);
        }
        else if (type == PowerUpType.DoublePoints && ui != null)
        {
            ui.scoreMultiplier = originalScoreRate;
        }

        carController.carSpeed = originalCarSpeed;
        if (track != null) track.speed = originalTrackSpeed;
    }

    public bool IsImmortal()
    {
        return isImmortal;
    }
}