using UnityEngine;
using System; 

public class EnemyCarMove : MonoBehaviour
{
    public float baseSpeed = 5f; 
    public float laneChangeSpeed = 2f; // Prędkość, z jaką auto zjeżdża na bok
    private float currentSpeed;
    private bool escapeMode = false;

    // --- Zmienne do zmiany pasa ---
    private bool isChangingLane = false;
    private float targetX;
    private float changeLaneTimer = 0f;

    public static Action<bool> OnEscapeModeChanged;

    private void OnEnable()
    {
        escapeMode = false;
        OnEscapeModeChanged += HandleEscapeMode;
        
        isChangingLane = false;
        // Losowy czas do pierwszego "przemyślenia", czy zmienić pas (od 1s do 3s po zespawnowaniu)
        changeLaneTimer = UnityEngine.Random.Range(1f, 3f); 
    }

    private void OnDisable()
    {
        OnEscapeModeChanged -= HandleEscapeMode;
    }

    private void HandleEscapeMode(bool isEscape)
    {
        escapeMode = isEscape;
    }

    private void Update()
    {
        currentSpeed = baseSpeed * FuzzyInferenceEngine.CurrentEnemySpeedMod;
        
        // 1. Ruch pionowy w dół (lub do góry przy power-upie Attack)
        Vector3 direction = escapeMode ? Vector3.up : Vector3.down;
        transform.Translate(direction * currentSpeed * Time.deltaTime, Space.World);

        // 2. Przetwarzanie zmiany pasa w poziomie
        if (!escapeMode)
        {
            HandleLaneChange();
        }
    }

    private void HandleLaneChange()
    {
        if (isChangingLane)
        {
            // Płynny przesuw na boki w osi X do wyznaczonego celu
            float step = laneChangeSpeed * Time.deltaTime;
            Vector3 newPos = transform.position;
            newPos.x = Mathf.MoveTowards(newPos.x, targetX, step);
            transform.position = newPos;

            // Jeśli dotarliśmy do celu, kończymy manewr i ustawiamy nowy timer
            if (Mathf.Abs(transform.position.x - targetX) < 0.05f)
            {
                isChangingLane = false;
                changeLaneTimer = UnityEngine.Random.Range(2f, 4f); 
            }
        }
        else
        {
            changeLaneTimer -= Time.deltaTime;
            if (changeLaneTimer <= 0f)
            {
                // Sprawdzamy szansę podaną przez AI (logikę rozmytą)
                if (UnityEngine.Random.value < FuzzyInferenceEngine.CurrentLaneChangeChance)
                {
                    StartLaneChange();
                }
                else
                {
                    // Spróbuje pomyśleć o tym znowu za chwilę
                    changeLaneTimer = UnityEngine.Random.Range(1.5f, 3f); 
                }
            }
        }
    }

    private void StartLaneChange()
    {
        isChangingLane = true;
        // Wybiera lewo lub prawo o szerokość ok. jednego pasa (1.2 jednostki)
        float moveDirection = UnityEngine.Random.value > 0.5f ? 1.2f : -1.2f;
        targetX = transform.position.x + moveDirection;
        
        // Zaciskamy klamrę, żeby wóz nie wyjechał na chodnik (na boki poza mapę)
        // Twoja mapa w spawnerze ma granicę +/- 2.1f
        targetX = Mathf.Clamp(targetX, -2.1f, 2.1f);
    }
}