using UnityEngine;

public class GameplayTuningLoader : MonoBehaviour
{
    [Header("Punkty zaczepienia na aucie (Top-Down)")]
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
        int selectedCarID = PlayerPrefs.GetInt("SelectedCarIndex", 0);
        Debug.LogWarning($"[GRA START] Wczytuję auto numer: {selectedCarID}");

        // ==========================================
        // 1. ŁADOWANIE SPOILERA
        // ==========================================
        string spoilerKey = "Car_" + selectedCarID + "_Spoiler_ID";
        int equippedSpoilerID = PlayerPrefs.GetInt(spoilerKey, 0);

        if (equippedSpoilerID == 0)
        {
            if (topSpoilerPoint != null) topSpoilerPoint.sprite = null;
            Debug.LogWarning("[GRA START] Auto nie ma spoilera. Zostawiam czystą klapę.");
            // BRAK "return;" - dzięki temu gra idzie czytać dalej!
        }
        else
        {
            bool spoilerFound = false;
            foreach (TuningPart part in allTuningParts)
            {
                if (part != null && part.partID == equippedSpoilerID)
                {
                    if (topSpoilerPoint != null) topSpoilerPoint.sprite = part.topDownSprite;
                    Debug.LogWarning("[GRA START] SUKCES! Nakładam spoiler: " + part.partName);
                    spoilerFound = true;
                    break; // Przerwij pętlę szukania i idź do zderzaka
                }
            }
            if (!spoilerFound) Debug.LogError($"[GRA START] BŁĄD: Pamięć mówi, że mam założyć spoiler ID {equippedSpoilerID}, ale nie ma go w bazie!");
        }

        // ==========================================
        // 2. ŁADOWANIE ZDERZAKA
        // ==========================================
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
                    if (FrontBumperPoint != null) FrontBumperPoint.sprite = part.topDownSprite;
                    Debug.LogWarning("[GRA START] SUKCES! Nakładam zderzak: " + part.partName);
                    bumperFound = true;
                    break; 
                }
            }
            if (!bumperFound) Debug.LogError($"[GRA START] BŁĄD: Brak zderzaka ID {equippedBumperID} w bazie All Tuning Parts!");
        }
    }
}