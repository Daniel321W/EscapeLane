using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CarSelectManager : MonoBehaviour
{
    [Header("Konfiguracja Aut")]
    public GameObject[] cars;
    public int[] carPrices;

    [Header("Menedżer Tuningu")]
    public TuningManager tuningManager; 

    [Header("UI Elementy")]
    public Button leftButton;
    public Button rightButton;
    public Button startButton;
    public Button backButton;      
    
    [Header("UI Sklepu")]
    public GameObject lockBanner;
    public Button buyButton;
    public Text priceText;
    public Text coinsText;

    private int currentIndex = 0;

    private void Start()
    {
        PlayerPrefs.SetInt("CarUnlocked_0", 1);
        PlayerPrefs.Save();

        leftButton.onClick.AddListener(() => ChangeCar(-1));
        rightButton.onClick.AddListener(() => ChangeCar(1));
        startButton.onClick.AddListener(PlayGame);
        buyButton.onClick.AddListener(BuyCar);
        
        if (backButton != null)
        {
            backButton.onClick.AddListener(LoadMenu);
        }

        foreach (var car in cars) car.SetActive(false);
        cars[currentIndex].SetActive(true);

        UpdateShopUI();

        if (tuningManager != null)
        {
            tuningManager.OnCarChanged(currentIndex, cars[currentIndex]);
        }
    }

    void ChangeCar(int direction)
    {
        cars[currentIndex].SetActive(false);
        currentIndex = (currentIndex + direction + cars.Length) % cars.Length;
        cars[currentIndex].SetActive(true);
        
        UpdateShopUI(); 

        if (tuningManager != null)
        {
            tuningManager.OnCarChanged(currentIndex, cars[currentIndex]);
        }
    }

    void UpdateShopUI()
    {
        bool isUnlocked = PlayerPrefs.GetInt("CarUnlocked_" + currentIndex, 0) == 1;
        int price = (currentIndex < carPrices.Length) ? carPrices[currentIndex] : 999999;

        if (coinsText != null)
        {
            coinsText.text = PlayerPrefs.GetInt("TotalPoints", 0).ToString();
        }

        if (isUnlocked)
        {
            lockBanner.SetActive(false);        
            buyButton.gameObject.SetActive(false);  
            startButton.gameObject.SetActive(true); 
            
            if (priceText != null) priceText.gameObject.SetActive(false);
        }
        else
        {
            lockBanner.SetActive(true);         
            buyButton.gameObject.SetActive(true);   
            startButton.gameObject.SetActive(false); 
            
            if (priceText != null)
            {
                priceText.gameObject.SetActive(true); 
                priceText.text = price.ToString() + "$"; 
            }
        }
    }

    public void BuyCar()
    {
        int price = (currentIndex < carPrices.Length) ? carPrices[currentIndex] : 999999;
        int currentMoney = PlayerPrefs.GetInt("TotalPoints", 0);

        if (currentMoney >= price)
        {
            currentMoney -= price;
            PlayerPrefs.SetInt("TotalPoints", currentMoney);
            PlayerPrefs.SetInt("CarUnlocked_" + currentIndex, 1);
            PlayerPrefs.Save();

            Debug.Log("Kupiono auto numer: " + currentIndex + " za cenę: " + price);
            UpdateShopUI();
        }
        else
        {
            Debug.LogWarning("Za mało kasy! Masz: " + currentMoney + ", a potrzeba: " + price);
        }
    }

    void PlayGame()
    {
        PlayerPrefs.SetInt("SelectedCarIndex", currentIndex);
        PlayerPrefs.Save();
        SceneManager.LoadScene("mapSelectScene");
    }

    void LoadMenu()
    {
        SceneManager.LoadScene("menuScene");
    }
}