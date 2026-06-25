using UnityEngine;

public class GameplayTuningLoader : MonoBehaviour
{
    [Header("Punkt zaczepienia na aucie (Top-Down)")]
    public SpriteRenderer topSpoilerPoint;
    public SpriteRenderer FrontBumperPoint;

    [Header("Baza wszystkich części")]
    public TuningPart[] allTuningParts; 

    void Start()
    {
        LoadEquippedParts();
    }

    private void LoadEquippedParts()
    {
        // 1. Pobieramy ID wybranego auta (którym gracz właśnie wjechał do gry)
        int selectedCarID = PlayerPrefs.GetInt("SelectedCarIndex", 0);

        // 2. Budujemy DYNAMICZNY klucz do pamięci dla TEGO KONKRETNEGO auta
        string saveKey = "Car_" + selectedCarID + "_Spoiler_ID";

        // 3. Sprawdzamy, jakie ID spoilera ma zapisane to konkretne auto
        int equippedSpoilerID = PlayerPrefs.GetInt(saveKey, 0);

        // --- INFO DO KONSOLI ---
        Debug.LogWarning($"[GRA START] Wczytuję auto numer: {selectedCarID}");
        Debug.LogWarning($"[GRA START] Szukam w pamięci klucza: {saveKey}");
        Debug.LogWarning($"[GRA START] Wczytane ID spoilera: {equippedSpoilerID}");

        // 4. Jeśli ID to 0, znaczy że auto ma być "czyste"
        if (equippedSpoilerID == 0)
        {
            if (topSpoilerPoint != null) topSpoilerPoint.sprite = null;
            Debug.LogWarning("[GRA START] Auto nie ma spoilera. Zostawiam czystą klapę.");
            return; 
        }

        string bumperKey = "Car_" + selectedCarID + "_Bumper_ID"; 
        int equippedBumperID = PlayerPrefs.GetInt(bumperKey, 0);

        if (equippedBumperID == 0)
        {
            if (FrontBumperPoint != null) FrontBumperPoint.sprite = null;
            Debug.LogWarning("[GRA START] Auto nie ma zderzaka. Zostawiam domyślny przód.");
        }
        else
        {
            bool bumperFound = false;
            foreach (TuningPart part in allTuningParts)
            {
                if (part != null && part.partID == equippedBumperID)
                {
                    // Ważne: nakładamy grafikę z widoku TopDown, a nie SideView!
                    if (FrontBumperPoint != null) FrontBumperPoint.sprite = part.topDownSprite;
                    Debug.LogWarning("[GRA START] SUKCES! Nakładam zderzak: " + part.partName);
                    bumperFound = true;
                    break; 
                }
            }
            if (!bumperFound) Debug.LogError($"[GRA START] BŁĄD: Brak zderzaka ID {equippedBumperID} w liście All Tuning Parts!");
        }

        // 5. Przeszukujemy naszą bazę w poszukiwaniu założonej części
        foreach (TuningPart part in allTuningParts)
        {
            if (part != null && part.partID == equippedSpoilerID)
            {
                if (topSpoilerPoint != null)
                {
                    topSpoilerPoint.sprite = part.topDownSprite;
                }
                Debug.LogWarning("[GRA START] SUKCES! Nakładam na auto: " + part.partName);
                return; 
            }
        }

        // Jeśli pętla nie znalazła części (np. zapomniałeś dodać jej do tablicy allTuningParts w Inspektorze)
        Debug.LogError($"[GRA START] BŁĄD: Pamięć mówi, że mam założyć spoiler ID {equippedSpoilerID}, ale nie ma go w liście All Tuning Parts!");
    }
}