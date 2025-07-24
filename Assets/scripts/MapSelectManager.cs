using UnityEngine;
using UnityEngine.SceneManagement;

public class MapSelectManager : MonoBehaviour
{
    public void SelectMap(string mapName)
    {
        PlayerPrefs.SetString("SelectedMap", mapName);
        SceneManager.LoadScene("level1"); // lub "gameScene"
    }
}
