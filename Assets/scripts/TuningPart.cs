using UnityEngine;

// Ta linijka pozwala tworzyć części z menu Unity
[CreateAssetMenu(fileName = "NewTuningPart", menuName = "Tuning/Część Samochodowa")]
public class TuningPart : ScriptableObject
{
    [Header("Informacje o części")]
    public int partID;              // Unikalny numer (np. 1 dla pierwszego spoilera)
    public string partName;         // Nazwa (np. "Spoiler Sportowy")
    public int price;               // Cena
    public PartType type;           // Typ części (wybierany z listy poniżej)

    [Header("Grafiki (Sprite)")]
    public Sprite sideViewSprite;   // Grafika do garażu (z boku)
    public Sprite topDownSprite;    // Grafika do gry (z góry) - może być puste, jeśli z góry nie widać
}

// Lista typów części (możesz tu w przyszłości dopisać więcej)
public enum PartType
{
    Spoiler,
    FrontBumper,
    Wheels
}