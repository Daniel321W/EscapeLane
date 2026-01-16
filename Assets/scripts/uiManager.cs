using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class uiManager : MonoBehaviour
{
    public Button[] buttons;
    public Text scoreText;

    private int _score;
    private bool _gameOver;
    public Text gameOverScoreText; 
    public Text highScoreText;     
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
        if (scoreText != null)
            scoreText.text = $"Score: {_score}";
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

        foreach (var button in buttons)
        {
            button.gameObject.SetActive(true);
        }

        
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (_score > highScore)
        {
            PlayerPrefs.SetInt("HighScore", _score);
            PlayerPrefs.Save(); 
        }

       
        if (gameOverScoreText != null)
        {
            gameOverScoreText.gameObject.SetActive(true);
            gameOverScoreText.text = "Your Score: " + _score;

            int savedHighScore = PlayerPrefs.GetInt("HighScore", 0);
            gameOverScoreText.text += "\nRekord: " + savedHighScore;
        }

        if (highScoreText != null)
        {
            highScoreText.text = "Record: " + highScore;
            highScoreText.gameObject.SetActive(true); 
        }

        if (scoreText != null)
            scoreText.gameObject.SetActive(false);

    }



    public void play()
    {
        SceneManager.LoadScene("level1");
    }

    public void play1()
    {
        SceneManager.LoadScene("level2");
    }

     public void play2()
    {
        SceneManager.LoadScene("level3");
    }

    public void selectmap()
    {
        SceneManager.LoadScene("mapSelectScene");
    }

    

    public void Pause()
    {
        Time.timeScale = Time.timeScale == 1 ? 0 : 1;
    }

    public void Menu()
    {
        SceneManager.LoadScene("menuScene");
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void Select()
    {
        SceneManager.LoadScene("carSelectScene");

    }



   





}
