using UnityEngine;

public class RandomGameplayMusic : MonoBehaviour
{
    public AudioClip[] gameplayTracks; 
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (gameplayTracks.Length > 0)
        {
            // Losowanie piosenki
            int randomIndex = Random.Range(0, gameplayTracks.Length);
            AudioClip selectedClip = gameplayTracks[randomIndex];

            audioSource.clip = selectedClip;
            audioSource.Play();

            // SZUKANIE UI I WYŚWIETLANIE POPUPU
            uiManager uiMan = FindObjectOfType<uiManager>();
            if (uiMan != null)
            {
                uiMan.ShowSongPopup(selectedClip.name);
            }
        }
    }
}