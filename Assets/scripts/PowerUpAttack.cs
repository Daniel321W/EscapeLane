using UnityEngine;
using static PowerUpManager;

public class PowerUpAttack : MonoBehaviour
{
    public float speed = 8f;

    private void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime, Space.World);
    }
//refactor
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PowerUpManager manager = other.GetComponent<PowerUpManager>();
            if (manager != null)
                manager.ActivatePowerUp(PowerUpType.Attack);

            Destroy(gameObject);
        }
    }
}
