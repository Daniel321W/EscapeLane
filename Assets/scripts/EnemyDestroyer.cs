using UnityEngine;

public class EnemyDestroyer : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("EnemyCar"))
        {
            Destroy(collision.gameObject);
        }
    }
}
