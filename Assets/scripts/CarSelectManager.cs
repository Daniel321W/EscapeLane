using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CarSelectManager : MonoBehaviour
{
    public GameObject[] cars; 
    public Button leftButton, rightButton, startButton;

    private int currentIndex = 0;

    private void Start()
    {
        UpdateCarVisibility();

        leftButton.onClick.AddListener(() => ChangeCar(-1));
        rightButton.onClick.AddListener(() => ChangeCar(1));
        startButton.onClick.AddListener(PlayGame);
    }

    void ChangeCar(int direction)
    {
        currentIndex = (currentIndex + direction + cars.Length) % cars.Length;
        UpdateCarVisibility();
    }

    void UpdateCarVisibility()
    {
        for (int i = 0; i < cars.Length; i++)
        {
            cars[i].SetActive(i == currentIndex);
        }
    }

    void PlayGame()
    {
        Debug.Log("Zapisuję wybrany samochód: " + currentIndex);
        PlayerPrefs.SetInt("SelectedCarIndex", currentIndex);
        PlayerPrefs.Save();
        SceneManager.LoadScene("mapSelectScene");
    }
}
