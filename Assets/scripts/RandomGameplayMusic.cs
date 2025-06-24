using UnityEngine;

public class RandomGameplayMusic : MonoBehaviour
{
    public AudioClip[] gameplayTracks; // wrzucasz tutaj utwory w Inspectorze
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (gameplayTracks.Length > 0)
        {
            int randomIndex = Random.Range(0, gameplayTracks.Length);
            audioSource.clip = gameplayTracks[randomIndex];
            audioSource.Play();
        }
    }
}
