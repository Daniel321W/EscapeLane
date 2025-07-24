using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerCarLoader : MonoBehaviour
{
    public GameObject[] playerCars; 
    public Vector3 spawnPosition = new Vector3(0f, -4f, 0f);
    public uiManager ui;
    public EventTrigger leftTrigger;
    public EventTrigger rightTrigger;



    void Start()
    {
        int selectedIndex = PlayerPrefs.GetInt("SelectedCarIndex", 0);
        Debug.Log("Liczba aut: " + playerCars.Length);
        Debug.Log("Wybrany index: " + selectedIndex);
        Debug.Log("Selected car index: " + selectedIndex);
        if (selectedIndex < 0 || selectedIndex >= playerCars.Length)
        {
            selectedIndex = 0;
            Debug.Log("Index poza zakresem, ustawiam na 0");
        }

        GameObject playerCar = Instantiate(playerCars[selectedIndex], spawnPosition, Quaternion.identity);

        carControler controller = playerCar.GetComponent<carControler>();
        if (controller != null)
        {
            controller.SetUIManager(ui);

            AddTrigger(leftTrigger, EventTriggerType.PointerDown, controller.PressLeftDown);
            AddTrigger(leftTrigger, EventTriggerType.PointerUp, controller.PressLeftUp);
            AddTrigger(rightTrigger, EventTriggerType.PointerDown, controller.PressRightDown);
            AddTrigger(rightTrigger, EventTriggerType.PointerUp, controller.PressRightUp);
        }
    }

    private void AddTrigger(EventTrigger trigger, EventTriggerType eventType, UnityEngine.Events.UnityAction action)
    {
        var entry = new EventTrigger.Entry { eventID = eventType };
        entry.callback.AddListener((eventData) => { action(); });
        trigger.triggers.Add(entry);
    }


}
