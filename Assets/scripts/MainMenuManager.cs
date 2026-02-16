using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Text coinsText; // Przeciągnij tutaj tekst z UI, który ma pokazywać walutę

    private void Start()
    {
        // Odśwież walutę przy starcie menu
        UpdateCoinsUI();
    }

   public void UpdateCoinsUI()
{
    if (coinsText != null)
    {
        int totalCoins = PlayerPrefs.GetInt("TotalPoints", 0);
        
        // Zamiast: coinsText.text = "Kasa: " + totalCoins;
        // Daj:
        coinsText.text = totalCoins.ToString();
    }
}
}