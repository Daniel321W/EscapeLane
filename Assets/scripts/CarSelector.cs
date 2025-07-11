using UnityEngine;
using UnityEngine.UI;

public class CarSelector : MonoBehaviour
{
    public GameObject[] cars;              // Samochody do wyboru (obiekty ze SpriteRendererem)
    public Sprite[] carNameSprites;        // Obrazki z nazwami aut
    public Image carNameImage;             // UI Image, gdzie wyświetli się obrazek z nazwą auta
    public Button buttonLeft;
    public Button buttonRight;

    private int currentIndex = 0;

    [Header("Auto Scaling Settings")]
    [Range(0.1f, 1f)] public float screenHeightRatio = 0.3f; // Auto zajmie np. 30% wysokości ekranu

    private void Start()
    {
        // 🔧 Skalowanie wszystkich aut przed pokazaniem jednego z nich
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

        // Zablokuj przyciski na skrajnych pozycjach
        buttonLeft.interactable = (index > 0);
        buttonRight.interactable = (index < cars.Length - 1);
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

        scale *= 1.1f; // 🔥 Tu zwiększasz wielkość auta

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