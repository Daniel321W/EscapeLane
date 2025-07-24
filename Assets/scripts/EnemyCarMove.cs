using UnityEngine;

public class EnemyCarMove : MonoBehaviour
{
    public float speed = 5f;
    private bool escapeMode = false;

    private void Update()
    {
        Vector3 direction = escapeMode ? Vector3.up : Vector3.down;
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    public void SetEscapeMode(bool value)
    {
        escapeMode = value;
    }
}
