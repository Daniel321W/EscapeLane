using UnityEngine;
using static PowerUpManager;

public class PowerUpTrigger : MonoBehaviour
{
    public PowerUpManager.PowerUpType type;  // np. DoublePoints
    public float speed = 2f;

     private void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) 
        {
            var powerUpManager = other.GetComponent<PowerUpManager>();
            if (powerUpManager != null)
            {
                powerUpManager.ActivatePowerUp(type);  
            }

            Destroy(gameObject); 
        }
    }
}
