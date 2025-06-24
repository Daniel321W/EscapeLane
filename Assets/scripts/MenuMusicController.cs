using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class MenuMusicController : MonoBehaviour
{
    private static MenuMusicController instance;
    private AudioSource _source;

    // ✨ Lista scen, w których ma grać muzyka menu
    private readonly string[] allowedScenes = { "menuScene", "carSelectScene", "mapSelectScene" };

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            _source = GetComponent<AudioSource>();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (IsSceneAllowed(scene.name))
        {
            if (!_source.isPlaying)
                _source.Play();
        }
        else
        {
            if (_source.isPlaying)
                _source.Stop();
        }
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
