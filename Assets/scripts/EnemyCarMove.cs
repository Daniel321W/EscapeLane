using UnityEngine;

public class EnemyCarMove : MonoBehaviour
{
    public float speed = 8f;

    private void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }
}
