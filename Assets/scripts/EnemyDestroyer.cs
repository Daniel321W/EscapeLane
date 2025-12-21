using UnityEngine;

public class EnemyDestroyer : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("EnemyCar"))
        {
            
            collision.gameObject.SetActive(false);
        }
    }
    
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyCar"))
        {
            collision.gameObject.SetActive(false);
        }
    }
}