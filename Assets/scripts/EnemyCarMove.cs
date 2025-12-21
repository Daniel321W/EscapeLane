using UnityEngine;
using System; 

public class EnemyCarMove : MonoBehaviour
{
    public float speed = 5f;
    private bool escapeMode = false;

    
    public static Action<bool> OnEscapeModeChanged;

    private void OnEnable()
    {
        
        escapeMode = false;
        
        OnEscapeModeChanged += HandleEscapeMode;
    }

    private void OnDisable()
    {
        OnEscapeModeChanged -= HandleEscapeMode;
    }

    private void HandleEscapeMode(bool isEscape)
    {
        escapeMode = isEscape;
    }

    private void Update()
    {
        Vector3 direction = escapeMode ? Vector3.up : Vector3.down;
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }
}