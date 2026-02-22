using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class MenuMusicController : MonoBehaviour
{
    private static MenuMusicController instance;
    private AudioSource _source;

    [Header("Playlista (Wrzuć tu piosenki)")]
    public AudioClip[] menuSongs; // ✨ Tablica na Twoje utwory

    // Lista scen, w których ma grać muzyka menu
    private readonly string[] allowedScenes = { "menuScene", "carSelectScene", "mapSelectScene" };

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            _source = GetComponent<AudioSource>();
            
            // WAŻNE: Odznaczamy "Loop" w AudioSource, żeby piosenka po zakończeniu
            // faktycznie się zatrzymała, co pozwoli skryptowi włączyć kolejną.
            _source.loop = false; 

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        
        if (IsSceneAllowed(SceneManager.GetActiveScene().name) && menuSongs.Length > 0)
        {
            if (!_source.isPlaying)
            {
                PlayRandomSong();
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (IsSceneAllowed(scene.name))
        {
            
            if (!_source.isPlaying && menuSongs.Length > 0)
            {
                PlayRandomSong();
            }
        }
        else
        {
            
            if (_source.isPlaying)
                _source.Stop();
        }
    }

    private void PlayRandomSong()
    {
        if (menuSongs.Length == 0) return; 

        
        int randomIndex = Random.Range(0, menuSongs.Length);
        
        
        _source.clip = menuSongs[randomIndex];
        _source.Play();
    }

    private bool IsSceneAllowed(string sceneName)
    {
        foreach (string allowed in allowedScenes)
        {
            if (sceneName == allowed)
                return true;
        }
        return false;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}