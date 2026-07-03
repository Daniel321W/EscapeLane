using UnityEngine;

public class NearMissZone : MonoBehaviour
{
    [Header("Referencja do głównego skryptu")]
    public NearMissManager manager;

    [Header("Ustawienia strefy")]
    public float zoneMultiplier = 1.0f; // Jaki mnożnik daje ta strefa (np. 0.5, 1.0, 2.0)
    public bool isFarZone = false;      // Zaznacz TRUE TYLKO dla największej strefy

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyCar"))
        {
            manager.RegisterOrUpdateMiss(collision.gameObject, zoneMultiplier);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Naliczamy nagrodę TYLKO wtedy, gdy auto całkowicie wyjeżdża z największej strefy (koniec manewru)
        if (isFarZone && collision.CompareTag("EnemyCar"))
        {
            manager.FinalizeMiss(collision.gameObject);
        }
    }
}