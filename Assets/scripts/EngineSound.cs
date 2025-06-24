using UnityEngine;

public class EngineSound : MonoBehaviour
{
    private AudioSource engineAudio;

    void Start()
    {
        engineAudio = GetComponent<AudioSource>();
        engineAudio.loop = true;
        engineAudio.Play();
    }
}
