using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public float speed = 8f; // Ruch w dół (jak przeciwnicy)

    private void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime, Space.World);

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PowerUpManager manager = other.GetComponent<PowerUpManager>();
            if (manager != null)
                manager.ActivatePowerUp();

            Destroy(gameObject);
        }
    }
}
