using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class uiManager : MonoBehaviour
{


    [Header("Near Miss UI")]
    public Text nearMissText; // Tylko jeden obiekt tekstu!

    public GameObject songPopupPanel; // Twój obrazek SD.jpg (Tło)
    public Text songNameText;         // Tekst z nazwą piosenki

    public void ShowSongPopup(string songName)
    {
        if (songPopupPanel != null && songNameText != null)
        {
            // Włączamy panel z grafiką
            songPopupPanel.SetActive(true);
            
            // Ustawiamy tekst (zmieniamy nazwę pliku z Unity na ładny napis)
            songNameText.text = "♫ " + songName;
            
            // Zniknie po 3 sekundach
            CancelInvoke(nameof(HideSongPopup)); 
            Invoke(nameof(HideSongPopup), 3.0f);
        }
    }

    private void HideSongPopup()
    {
        if (songPopupPanel != null)
        {
            songPopupPanel.SetActive(false);
        }
    }

    public void ShowNearMissText(float multiplier)
    {
        if (nearMissText != null)
        {
            // Włączamy tekst
            nearMissText.gameObject.SetActive(true);
            
            // Formatujemy napis, np.: "NEAR MISS! x2.0"
            nearMissText.text = "NEAR MISS x" + multiplier.ToString("F1"); 
            
            // Zniknie po 1.5 sekundy
            CancelInvoke(nameof(HideNearMissText)); 
            Invoke(nameof(HideNearMissText), 1.5f);
        }
    }

    private void HideNearMissText()
    {
        if (nearMissText != null)
        {
            nearMissText.gameObject.SetActive(false);
        }
    }
    public Button[] buttons;
    public Text scoreText;

    private int _score;
    private bool _gameOver;
    public Text gameOverScoreText; 
    public Text highScoreText;     
    
    // NOWE: Tekst do wyświetlania całkowitej ilości waluty na ekranie Game Over
    public Text totalPointsText; 

    [HideInInspector]
    public float scoreMultiplier = 1f;

    private void Start()
    {
        _score = 0;
        _gameOver = false;
        InvokeRepeating(nameof(UpdateScore), 1f, 0.5f);

        if (highScoreText != null)
        {
            int highScore = PlayerPrefs.GetInt("HighScore", 0);
            highScoreText.text = "Best: " + highScore;
        }
    }

    private void Update()
    {
        if (scoreText != null && !_gameOver)
            scoreText.text = _score.ToString();
    }

    private void UpdateScore()
    {
        if (!_gameOver)
        {
            _score += Mathf.RoundToInt(1 * scoreMultiplier);
        }
    }

    // DODAJ TĘ FUNKCJĘ DO SWOJEGO uiManager:
    public void AddScore(int pointsToAdd)
    {
        if (!_gameOver)
        {
            _score += pointsToAdd;
        }
    }

    public void gameOverActivated()
    {
        _gameOver = true;

        // Pokazanie przycisków
        foreach (var button in buttons)
        {
            button.gameObject.SetActive(true);
        }
        
        // --- 1. ZAPISYWANIE REKORDU (HIGH SCORE) ---
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (_score > highScore)
        {
            PlayerPrefs.SetInt("HighScore", _score);
            PlayerPrefs.Save(); 
            highScore = _score; // Aktualizujemy zmienną
        }

        // --- 2. SYSTEM WALUTY ---
        // Pobieramy aktualny stan konta
        int currentTotalPoints = PlayerPrefs.GetInt("TotalPoints", 0);
        
        // Dodajemy punkty z tego biegu
        currentTotalPoints += _score;
        
        // Zapisujemy nowy stan konta
        PlayerPrefs.SetInt("TotalPoints", currentTotalPoints);
        PlayerPrefs.Save();


        // --- 3. AKTUALIZACJA UI NA EKRANIE GAME OVER ---
        if (gameOverScoreText != null)
        {
            gameOverScoreText.gameObject.SetActive(true);
            
            // POPRAWIONA LINIA (usunięty średnik w środku):
            gameOverScoreText.text = _score.ToString() + "\nRekord: " + highScore;
        }

        if (highScoreText != null)
        {
            highScoreText.text = "Record: " + highScore;
            highScoreText.gameObject.SetActive(true); 
        }

        // NOWE: Wyświetlanie portfela (sama liczba, bo masz ikonkę)
        if (totalPointsText != null)
        {
            totalPointsText.gameObject.SetActive(true);
            // Zmienione na samą liczbę (bez słowa "Wallet"), żeby pasowało do ikonki
            totalPointsText.text = currentTotalPoints.ToString(); 
        }

        if (scoreText != null)
            scoreText.gameObject.SetActive(false);
    }

   
    public void play() { SceneManager.LoadScene("level1"); }
    public void play1() { SceneManager.LoadScene("level2"); }
    public void play2() { SceneManager.LoadScene("level3"); }
    public void selectmap() { SceneManager.LoadScene("mapSelectScene"); }
    public void Pause() { Time.timeScale = Time.timeScale == 1 ? 0 : 1; }
    public void Menu() { SceneManager.LoadScene("menuScene"); }
    public void Exit() { Application.Quit(); }
    public void Select() { SceneManager.LoadScene("carSelectScene"); }
}