using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CarSelectManager : MonoBehaviour
{
    [Header("Konfiguracja Aut")]
    public GameObject[] cars;       // Tablica z modelami aut
    public int[] carPrices;         // Tablica cen (musi być tyle samo co aut!)

    [Header("UI Elementy")]
    public Button leftButton;
    public Button rightButton;
    public Button startButton;      // Przycisk "Jazda/Dalej" (widoczny tylko jak kupione)
    
    [Header("UI Sklepu")]
    public GameObject lockBanner;   // Obiekt kłódki/banera
    public Button buyButton;        // Przycisk kupowania
    public Text priceText;          // Tekst ceny (np. "5000")
    public Text coinsText;          // Tekst portfela w rogu ekranu

    private int currentIndex = 0;

    private void Start()
    {
        // Zawsze odblokuj pierwsze auto (indeks 0), żeby gracz nie utknął
        PlayerPrefs.SetInt("CarUnlocked_0", 1);
        PlayerPrefs.Save();

        // Podpięcie przycisków
        leftButton.onClick.AddListener(() => ChangeCar(-1));
        rightButton.onClick.AddListener(() => ChangeCar(1));
        startButton.onClick.AddListener(PlayGame);
        buyButton.onClick.AddListener(BuyCar);

        // Wyłączamy wszystkie auta na start, żeby nie nakładały się na siebie
        foreach (var car in cars) car.SetActive(false);
        cars[currentIndex].SetActive(true);

        UpdateShopUI(); // Pierwsze odświeżenie widoku
    }

    void ChangeCar(int direction)
    {
        cars[currentIndex].SetActive(false); // Wyłącz stare auto
        
        // Oblicz nowy indeks (zapętlanie listy)
        currentIndex = (currentIndex + direction + cars.Length) % cars.Length;
        
        cars[currentIndex].SetActive(true); // Włącz nowe auto
        
        UpdateShopUI(); // KLUCZOWE: Odśwież cenę i kłódkę dla NOWEGO auta
    }

    void UpdateShopUI()
    {
        // 1. Sprawdź, czy auto jest odblokowane
        bool isUnlocked = PlayerPrefs.GetInt("CarUnlocked_" + currentIndex, 0) == 1;

        // 2. Pobierz cenę AKTUALNEGO auta
        // Zabezpieczenie: jeśli zapomnisz wpisać ceny w inspektorze, ustawi 999999
        int price = (currentIndex < carPrices.Length) ? carPrices[currentIndex] : 999999;

        // 3. Aktualizacja Portfela (zawsze pokazuj aktualny stan konta)
        if (coinsText != null)
        {
            coinsText.text = PlayerPrefs.GetInt("TotalPoints", 0).ToString();
        }

        // 4. Logika widoczności (Kupione vs Zablokowane)
        if (isUnlocked)
        {
            // --- AUTO JUŻ MAMY ---
            lockBanner.SetActive(false);        // Ukryj kłódkę
            buyButton.gameObject.SetActive(false);  // Ukryj przycisk kupowania
            startButton.gameObject.SetActive(true); // Pokaż przycisk startu
            
            // WAŻNE: Ukryj cenę, bo po co pokazywać cenę kupionego auta?
            if (priceText != null) priceText.gameObject.SetActive(false);
        }
        else
        {
            // --- AUTO TRZEBA KUPIĆ ---
            lockBanner.SetActive(true);         // Pokaż kłódkę
            buyButton.gameObject.SetActive(true);   // Pokaż przycisk kupowania
            startButton.gameObject.SetActive(false); // Ukryj start
            
            // WAŻNE: Pokaż cenę i zaktualizuj jej treść dla TEGO konkretnego auta
            if (priceText != null)
            {
                priceText.gameObject.SetActive(true); // Pokaż tekst
                priceText.text = price.ToString();    // Wpisz nową cenę (np. 5000)
            }
        }
    }

    public void BuyCar()
    {
        int price = (currentIndex < carPrices.Length) ? carPrices[currentIndex] : 999999;
        int currentMoney = PlayerPrefs.GetInt("TotalPoints", 0);

        if (currentMoney >= price)
        {
            // Odejmij pieniądze
            currentMoney -= price;
            PlayerPrefs.SetInt("TotalPoints", currentMoney);

            // Odblokuj to konkretne auto
            PlayerPrefs.SetInt("CarUnlocked_" + currentIndex, 1);
            PlayerPrefs.Save();

            Debug.Log("Kupiono auto numer: " + currentIndex + " za cenę: " + price);

            // Odśwież widok (teraz kłódka i cena znikną, pojawi się Start)
            UpdateShopUI();
        }
        else
        {
            Debug.Log("Za mało kasy! Masz: " + currentMoney + ", a potrzeba: " + price);
            // Tu możesz dodać np. dźwięk błędu
        }
    }

    void PlayGame()
    {
        PlayerPrefs.SetInt("SelectedCarIndex", currentIndex);
        PlayerPrefs.Save();
        SceneManager.LoadScene("mapSelectScene");
    }
}