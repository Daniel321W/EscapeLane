using UnityEngine;
using UnityEngine.UI;

public class carControler : MonoBehaviour
{
    [Header("Ustawienia")]
    public float carSpeed;
    public float maxPos = 2.1f;
    
    [Header("Komponenty")]
    public Button[] buttons;
    public uiManager ui;

    // Zmienne sterujące
    private bool moveLeft;
    private bool moveRight;
    private float currentSpeedX;

    private Vector3 _position;
    private Rigidbody2D rb;
    private PowerUpManager powerUpManager;

    private void Start()
    {
        _position = transform.position;
        rb = GetComponent<Rigidbody2D>();
        powerUpManager = GetComponent<PowerUpManager>();

        // ZABEZPIECZENIE: Jeśli masz Rigidbody, zerujemy grawitację, 
        // żeby nie przeszkadzała w sterowaniu ręcznym
        if (rb != null)
        {
            rb.gravityScale = 0;
            rb.linearVelocity = Vector2.zero; 
        }
    }

    private void Update()
    {
        // 1. Zawsze aktualizujemy pozycję na początku klatki
        _position = transform.position;

        // 2. Obliczamy przesunięcie w tej klatce
        // Używamy carSpeed, który jest modyfikowany przez PowerUpManager
        float moveDelta = carSpeed * Time.deltaTime; 

        if (moveLeft)
        {
            _position.x -= moveDelta;
        }
        else if (moveRight)
        {
            _position.x += moveDelta;
        }

        // 3. Ograniczenie (Clamp) - żeby nie wyjechać za ekran
        _position.x = Mathf.Clamp(_position.x, -maxPos, maxPos);

        // 4. Przypisanie nowej pozycji
        transform.position = _position;
        
        // 5. Zerowanie fizyki (ważne!)
        // Ponieważ ruszamy autem ręcznie (Transform), musimy upewnić się, 
        // że fizyka nie próbuje nim rzucać w innym kierunku po kolizji.
        if (rb != null) rb.linearVelocity = Vector2.zero;
    }

    // --- KOLIZJE ---
    // Mimo że sterujemy pozycją, OnCollisionEnter2D nadal zadziała, 
    // dopóki obiekt ma Rigidbody i Collider.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("EnemyCar"))
        {
            if (powerUpManager != null && powerUpManager.IsImmortal())
            {
                collision.gameObject.SetActive(false);
            }
            else
            {
                Destroy(gameObject);
                if (ui != null)
                    ui.gameOverActivated();
            }
        }
    }

    public void SetUIManager(uiManager manager)
    {
        ui = manager;
    }

    // --- STEROWANIE DOTYKOWE (Event Triggers) ---

    public void PressLeftDown() 
    { 
        moveLeft = true; 
        moveRight = false; // Zabezpieczenie przed wciśnięciem obu naraz
    }
    
    public void PressLeftUp() 
    { 
        moveLeft = false; 
    }

    public void PressRightDown() 
    { 
        moveRight = true; 
        moveLeft = false; // Zabezpieczenie
    }

    public void PressRightUp() 
    { 
        moveRight = false; 
    }

    // Ta funkcja jest teraz opcjonalna, bo zerujemy velocity w Update,
    // ale zostawiam ją dla kompatybilności z Twoim UI
    public void SetVelocityZero()
    {
        if (rb != null) rb.linearVelocity = Vector2.zero;
    }
}