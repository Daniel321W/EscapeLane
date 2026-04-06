using UnityEngine;
using UnityEngine.UI;

public class CarSelector : MonoBehaviour
{
    public GameObject[] cars;              // Samochody do wyboru (obiekty ze SpriteRendererem)
    public Sprite[] carNameSprites;        // Obrazki z nazwami aut
    public Image carNameImage;             // UI Image, gdzie wyświetli się obrazek z nazwą auta
    public Button buttonLeft;
    public Button buttonRight;

    // NOWE: Połączenie z naszym systemem sklepu
    public TuningManager tuningManager; 

    private int currentIndex = 0;

    [Header("Auto Scaling Settings")]
    [Range(0.1f, 1f)] public float screenHeightRatio = 0.3f; 

    private void Start()
    {
        foreach (GameObject car in cars)
        {
            ScaleCarToScreen(car);
        }

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

        buttonLeft.interactable = (index > 0);
        buttonRight.interactable = (index < cars.Length - 1);

        // NOWE: Informujemy sklep, że zmieniliśmy auto i przekazujemy mu obiekt nowego auta
        if (tuningManager != null)
        {
            tuningManager.OnCarChanged(index, cars[index]);
        }
    }

    private void ScaleCarToScreen(GameObject carObject)
    {
        SpriteRenderer sr = carObject.GetComponent<SpriteRenderer>();
        if (sr == null || Camera.main == null) return;

        float spriteWidth = sr.sprite.bounds.size.x;
        float spriteHeight = sr.sprite.bounds.size.y;

        float worldScreenHeight = Camera.main.orthographicSize * 2f;
        float worldScreenWidth = worldScreenHeight * Screen.width / Screen.height;

        float maxHeight = worldScreenHeight * screenHeightRatio;
        float maxWidth = worldScreenWidth * 0.8f;

        float scale = Mathf.Min(maxWidth / spriteWidth, maxHeight / spriteHeight);

        scale *= 1.1f; 

        carObject.transform.localScale = new Vector3(scale, scale, 1f);
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