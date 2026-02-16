using UnityEngine;
using UnityEngine.SceneManagement;

public class CheatManager : MonoBehaviour
{
    void Update()
    {
       
        if (Input.GetKeyDown(KeyCode.M))
        {
            int currentMoney = PlayerPrefs.GetInt("TotalPoints", 0);
            PlayerPrefs.SetInt("TotalPoints", currentMoney + 1000);
            PlayerPrefs.Save();
            Debug.Log("CHEAT: Dodano 1000 kasy. Masz teraz: " + (currentMoney + 1000));
            
            
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        
        if (Input.GetKeyDown(KeyCode.R))
        {
            PlayerPrefs.DeleteAll(); 
            PlayerPrefs.Save();
            Debug.Log("CHEAT: Zresetowano cały postęp!");
            

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (Input.GetKeyDown(KeyCode.U))
        {

            for (int i = 0; i < 10; i++)
            {
                PlayerPrefs.SetInt("CarUnlocked_" + i, 1);
            }
            PlayerPrefs.Save();
            Debug.Log("CHEAT: Wszystkie auta odblokowane!");
            
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}