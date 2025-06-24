using UnityEngine;

public class PersistentMusic : MonoBehaviour
{
    private static PersistentMusic instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // ✨ NIE usuwaj między scenami
        }
        else
        {
            Destroy(gameObject); // 🚫 Jeśli już istnieje, nie duplikuj
        }
    }
}
