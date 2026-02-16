using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class uiManager : MonoBehaviour
{
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