using UnityEngine;
using UnityEngine.UI;

public class carControler : MonoBehaviour
{
    public Button[] buttons;
    public float carSpeed;
    public float maxPos = 2.1f;
    public uiManager ui;

    private Vector3 _position;

    private bool moveLeft;
    private bool moveRight;

    private Rigidbody2D rb;
    private PowerUpManager powerUpManager;

    private void Start()
    {
        _position = transform.position;
        rb = GetComponent<Rigidbody2D>();
        powerUpManager = GetComponent<PowerUpManager>();
    }

    private void Update()
    {
        _position = transform.position;

        float moveX = 0f;

        if (moveLeft)
            moveX = -carSpeed * Time.deltaTime;
        else if (moveRight)
            moveX = carSpeed * Time.deltaTime;
        else
            moveX = Input.GetAxis("Horizontal") * carSpeed * Time.deltaTime;

        _position.x += moveX;
        _position.x = Mathf.Clamp(_position.x, -maxPos, maxPos);
        transform.position = _position;
    }

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

    public void PressLeftDown() { moveLeft = true; }
    public void PressLeftUp() { moveLeft = false; }

    public void PressRightDown()
    {
        
        if (rb != null) 
            rb.linearVelocity = new Vector2(carSpeed, 0);
    }

    public void PressRightUp()
    {
        if (rb != null) 
            rb.linearVelocity = Vector2.zero;
    }

    public void SetVelocityZero()
    {
        if (rb != null)
            rb.linearVelocity = Vector2.zero;
    }
}