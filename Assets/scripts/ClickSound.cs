using UnityEngine;

public class ClickSound : MonoBehaviour
{
    public AudioSource audioSource;

    public void PlayClick()
    {
        if (audioSource != null)
            audioSource.Play();
    }
}
