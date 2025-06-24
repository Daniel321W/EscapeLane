using UnityEngine;
using UnityEngine.UI;

public class CarSelector : MonoBehaviour
{
    public GameObject[] cars;              // Samochody do wyboru
    public Sprite[] carNameSprites;        // Obrazki z nazwami aut
    public Image carNameImage;             // UI Image, gdzie wyświetli się obrazek z nazwą auta
    public Button buttonLeft;
    public Button buttonRight;

    private int currentIndex = 0;

    private void Start()
    {
        ShowCar(currentIndex);

        buttonLeft.onClick.AddListener(OnLeftClick);
        buttonRight.onClick.AddListener(OnRightClick);
    }

    private void ShowCar(int index)
    {
        // Aktywuj tylko wybrany samochód
        for (int i = 0; i < cars.Length; i++)
        {
            cars[i].SetActive(i == index);
        }

        // Zmień obrazek z nazwą auta
        if (index >= 0 && index < carNameSprites.Length)
        {
            carNameImage.sprite = carNameSprites[index];
        }

        // Zablokuj przyciski na skrajnych pozycjach
        buttonLeft.interactable = (index > 0);
        buttonRight.interactable = (index < cars.Length - 1);
    }

    private void OnLeftClick()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            ShowCar(currentIndex);
        }
    }

    private void OnRightClick()
    {
        if (currentIndex < cars.Length - 1)
        {
            currentIndex++;
            ShowCar(currentIndex);
        }
    }

    public GameObject GetSelectedCar()
    {
        return cars[currentIndex];
    }
}
