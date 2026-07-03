using System.Collections.Generic;
using UnityEngine;

public class NearMissManager : MonoBehaviour
{
    private Dictionary<GameObject, float> activeNearMisses = new Dictionary<GameObject, float>();

    [Header("Ustawienia nagród (ile dodaje do waluty/score)")]
    public float basePoints = 50f; // Ile bazowo dodajemy do Twojej waluty

    // Referencja do Twojego skryptu UI
    private uiManager uiMan;

    private void Start()
    {
        // Skrypt sam szuka uiManagera na scenie, nie musisz nic przeciągać!
        uiMan = FindObjectOfType<uiManager>();
    }

    public void RegisterOrUpdateMiss(GameObject enemy, float multiplier)
    {
        if (!activeNearMisses.ContainsKey(enemy))
        {
            activeNearMisses.Add(enemy, multiplier);
        }
        else
        {
            if (multiplier > activeNearMisses[enemy])
            {
                activeNearMisses[enemy] = multiplier;
            }
        }
    }

    public void FinalizeMiss(GameObject enemy)
    {
        if (activeNearMisses.ContainsKey(enemy))
        {
            float finalMultiplier = activeNearMisses[enemy];
            
            // Obliczamy ile waluty się należy (np. 50 pkt * 2.0 = 100 waluty)
            int totalReward = Mathf.RoundToInt(basePoints * finalMultiplier);

            Debug.LogWarning($"[Near Miss] SUKCES! Zgarniasz: {totalReward} waluty (Mnożnik: x{finalMultiplier})");

           // --- PRZELEWAMY WALUTĘ I POKAZUJEMY TEKST ---
            if (uiMan != null)
            {
                uiMan.AddScore(totalReward);
                uiMan.ShowNearMissText(finalMultiplier); // <--- Zaktualizowana nazwa
            }

            activeNearMisses.Remove(enemy);
        }
    }

    public void CancelMiss(GameObject enemy)
    {
        if (activeNearMisses.ContainsKey(enemy))
        {
            activeNearMisses.Remove(enemy);
        }
    }
}