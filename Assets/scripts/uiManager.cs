using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class uiManager : MonoBehaviour
{
    public Button[] buttons;
    public Text scoreText;

    private int _score;
    private bool _gameOver;
    

    private void Start()
    {
        _score = 0;
        _gameOver = false;
        InvokeRepeating(nameof(UpdateScore), 1f, 0.5f);
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
            _score++;
        }
    }

    public void gameOverActivated()
    {
        _gameOver = true;
        foreach (var button in buttons)
        {
            button.gameObject.SetActive(true);
        }
    }

    public void play()
    {
        SceneManager.LoadScene("level1");
    }

    public void play1()
    {
        SceneManager.LoadScene("level2");
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
