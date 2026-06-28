using UnityEngine;
using UnityEngine.UI;

public class TuningManager : MonoBehaviour
{
    [Header("Baza wszystkich części")]
    public TuningPart[] allSpoilers; 
    public TuningPart[] allWheels;    
    public TuningPart[] allBumpers;  
    
    [Header("Zakładki (Tabs)")]
    public Button tabAeroButton;        
    public Button tabWheelsButton;    
    public Button tabBumperButton;  

    [Header("Shop UI Settings")]
    public GameObject shopPanel;        
    public GameObject partButtonPrefab; 
    public Transform scrollContent;     
    public Text moneyText;              

    [Header("Big Action Button")]
    public Button actionBottomButton;   
    public Text actionBottomButtonText; 

    private SpriteRenderer currentCarSpoilerPoint;
    private SpriteRenderer currentCarBumperPoint;
    private SpriteRenderer currentCarFrontWheelPoint; 
    private SpriteRenderer currentCarRearWheelPoint;
    private int currentCarID = 0;
    
    private TuningPart currentlySelectedPart = null; 
    private int currentCategoryIndex = 0; 

    private void Start()
    {
        UpdateMoneyUI();
        CheckCarOwnershipAndToggleShop(); 
        
        if(actionBottomButton != null)
        {
            actionBottomButton.onClick.AddListener(OnMainActionClicked);
        }

        if(tabAeroButton != null)
            tabAeroButton.onClick.AddListener(() => ChangeCategory(0));
            
        if(tabWheelsButton != null)
            tabWheelsButton.onClick.AddListener(() => ChangeCategory(1));

        if(tabBumperButton != null)
            tabBumperButton.onClick.AddListener(() => ChangeCategory(2));
    }

    public void ChangeCategory(int newCategoryIndex)
    {
        currentCategoryIndex = newCategoryIndex;
        DeselectPart(); 
        GenerateShopUI(); 
    }

    public void OnCarChanged(int newCarID, GameObject currentCarObject)
    {
        currentCarID = newCarID;
        
        Transform spoilerTransform = currentCarObject.transform.Find("SpoilerPoint");
        currentCarSpoilerPoint = (spoilerTransform != null) ? spoilerTransform.GetComponent<SpriteRenderer>() : null;

        Transform bumperTransform = currentCarObject.transform.Find("FrontBumperPoint");
        currentCarBumperPoint = (bumperTransform != null) ? bumperTransform.GetComponent<SpriteRenderer>() : null;

        Transform frontWheelsTransform = currentCarObject.transform.Find("FrontWheelsPoint");
        currentCarFrontWheelPoint = (frontWheelsTransform != null) ? frontWheelsTransform.GetComponent<SpriteRenderer>() : null;

        Transform rearWheelsTransform = currentCarObject.transform.Find("RearWheelsPoint");
        currentCarRearWheelPoint = (rearWheelsTransform != null) ? rearWheelsTransform.GetComponent<SpriteRenderer>() : null;

        Debug.LogWarning($"--- ZMIANA AUTA --- Auto ID: {currentCarID} | Nazwa obiektu: {currentCarObject.name} | Spoiler: {currentCarSpoilerPoint != null} | Zderzak: {currentCarBumperPoint != null} | Koła: {currentCarFrontWheelPoint != null}");

        LoadTuning();
        CheckCarOwnershipAndToggleShop();
        DeselectPart(); 
    }

    private void CheckCarOwnershipAndToggleShop()
    {
        bool isOwned = (currentCarID == 0) || (PlayerPrefs.GetInt("CarUnlocked_" + currentCarID, 0) == 1);
        if (shopPanel != null) shopPanel.SetActive(isOwned); 
        if (actionBottomButton != null) actionBottomButton.gameObject.SetActive(isOwned);

        if (isOwned)
        {
            GenerateShopUI();
        }
    }

    private void GenerateShopUI()
    {
        foreach (Transform child in scrollContent)
        {
            Destroy(child.gameObject);
        }

        TuningPart[] activeDatabase = null;
        if (currentCategoryIndex == 0) activeDatabase = allSpoilers;
        else if (currentCategoryIndex == 1) activeDatabase = allWheels;
        else if (currentCategoryIndex == 2) activeDatabase = allBumpers;

        if (activeDatabase == null) return;

        if (currentCategoryIndex == 0 && currentCarSpoilerPoint == null) return; 
        if (currentCategoryIndex == 2 && currentCarBumperPoint == null) return; 
        if (currentCategoryIndex == 1 && (currentCarFrontWheelPoint == null || currentCarRearWheelPoint == null)) return; 

        foreach (TuningPart part in activeDatabase)
        {
            if (part == null) continue;

            GameObject newButton = Instantiate(partButtonPrefab, scrollContent);
            
            Image[] images = newButton.GetComponentsInChildren<Image>();
            if(images.Length > 1 && part.sideViewSprite != null) {
                images[1].sprite = part.sideViewSprite; 
                images[1].preserveAspect = true; 
            }

            Text btnText = newButton.GetComponentInChildren<Text>();
            if (btnText == null) continue;
            
            string ownedKey = "Car_" + currentCarID + "_Cat_" + currentCategoryIndex + "_Owned_Part_" + part.partID;
            bool isOwned = PlayerPrefs.GetInt(ownedKey, 0) == 1;

            if (isOwned) {
                btnText.text = "OWNED";
                btnText.color = Color.green; 
            } else {
                btnText.text = "$" + part.price;
                btnText.color = Color.yellow; 
            }

            Button btnComponent = newButton.GetComponent<Button>();
            if (btnComponent != null) {
                btnComponent.onClick.AddListener(() => SelectPart(part));
            }
        }
    }

    private void DeselectPart()
    {
        currentlySelectedPart = null;
        if(actionBottomButton != null)
        {
            if ((currentCategoryIndex == 0 && currentCarSpoilerPoint == null) ||
                (currentCategoryIndex == 1 && (currentCarFrontWheelPoint == null || currentCarRearWheelPoint == null)) ||
                (currentCategoryIndex == 2 && currentCarBumperPoint == null))
            {
                actionBottomButtonText.text = "NOT AVAILABLE FOR THIS CAR";
                actionBottomButtonText.color = Color.gray; 
                actionBottomButton.interactable = false;
            }
            else
            {
                actionBottomButtonText.text = "SELECT PART";
                actionBottomButtonText.color = Color.white; 
                actionBottomButton.interactable = false; 
            }
        }
    }

    public void SelectPart(TuningPart clickedPart)
    {
        currentlySelectedPart = clickedPart;
        
        string ownedKey = "Car_" + currentCarID + "_Cat_" + currentCategoryIndex + "_Owned_Part_" + clickedPart.partID;
        bool isPartOwned = PlayerPrefs.GetInt(ownedKey, 0) == 1;

        actionBottomButton.interactable = true; 

        string saveKey = "";
        if (currentCategoryIndex == 0) saveKey = "Car_" + currentCarID + "_Spoiler_ID";
        else if (currentCategoryIndex == 1) saveKey = "Car_" + currentCarID + "_Wheels_ID";
        else if (currentCategoryIndex == 2) saveKey = "Car_" + currentCarID + "_Bumper_ID";

        int currentlyEquippedID = PlayerPrefs.GetInt(saveKey, 0);

        if (clickedPart.partID == currentlyEquippedID)
        {
            actionBottomButtonText.text = "UNEQUIP " + clickedPart.partName;
            actionBottomButtonText.color = Color.red;
        }
        else if (isPartOwned)
        {
            actionBottomButtonText.text = "EQUIP " + clickedPart.partName;
            actionBottomButtonText.color = Color.white;
        }
        else
        {
            actionBottomButtonText.text = "BUY " + clickedPart.partName + " ($" + clickedPart.price + ")";
            actionBottomButtonText.color = Color.white;
        }
    }

    public void OnMainActionClicked()
    {
        if (currentlySelectedPart == null) return; 

        string saveKey = "";
        if (currentCategoryIndex == 0) saveKey = "Car_" + currentCarID + "_Spoiler_ID";
        else if (currentCategoryIndex == 1) saveKey = "Car_" + currentCarID + "_Wheels_ID";
        else if (currentCategoryIndex == 2) saveKey = "Car_" + currentCarID + "_Bumper_ID";

        int currentlyEquippedID = PlayerPrefs.GetInt(saveKey, 0);


        if (currentlySelectedPart.partID == currentlyEquippedID)
        {
            UnequipPart();
            return;
        }

        string ownedKey = "Car_" + currentCarID + "_Cat_" + currentCategoryIndex + "_Owned_Part_" + currentlySelectedPart.partID;
        bool isPartOwned = PlayerPrefs.GetInt(ownedKey, 0) == 1;

        if (isPartOwned)
        {
            EquipPart(currentlySelectedPart); 
        }
        else
        {
            int currentMoney = PlayerPrefs.GetInt("TotalPoints", 0); 
            if (currentMoney >= currentlySelectedPart.price)
            {
                currentMoney -= currentlySelectedPart.price;
                PlayerPrefs.SetInt("TotalPoints", currentMoney); 
                PlayerPrefs.SetInt(ownedKey, 1); 
                
                EquipPart(currentlySelectedPart); 
                UpdateMoneyUI();        
                
                GenerateShopUI(); 
                SelectPart(currentlySelectedPart); 
            }
            else
            {
                Debug.LogWarning("Not enough money!");
            }
        }
    }


    private void UnequipPart()
    {
        if (currentCategoryIndex == 0) 
        {
            if (currentCarSpoilerPoint != null) currentCarSpoilerPoint.sprite = null;
            PlayerPrefs.SetInt("Car_" + currentCarID + "_Spoiler_ID", 0);
            Debug.LogWarning($"[ZDEJMOWANIE] Ściągnięto spoiler z auta {currentCarID}. Zapisano ID: 0");
        }
        else if (currentCategoryIndex == 1) 
        {
            if (currentCarFrontWheelPoint != null) currentCarFrontWheelPoint.sprite = null;
            if (currentCarRearWheelPoint != null) currentCarRearWheelPoint.sprite = null;
            PlayerPrefs.SetInt("Car_" + currentCarID + "_Wheels_ID", 0);
            Debug.LogWarning($"[ZDEJMOWANIE] Ściągnięto tuningowe koła z auta {currentCarID}. Wracam do stocku.");
        }
        else if (currentCategoryIndex == 2) 
        {
            if (currentCarBumperPoint != null) currentCarBumperPoint.sprite = null;
            PlayerPrefs.SetInt("Car_" + currentCarID + "_Bumper_ID", 0);
            Debug.LogWarning($"[ZDEJMOWANIE] Ściągnięto zderzak z auta {currentCarID}. Zapisano ID: 0");
        }

        PlayerPrefs.Save();
        

        SelectPart(currentlySelectedPart); 
    }

    private void EquipPart(TuningPart partToEquip)
    {
        if (currentCategoryIndex == 0) 
        {
            if (currentCarSpoilerPoint == null) return; 

            currentCarSpoilerPoint.sprite = partToEquip.sideViewSprite;
            string saveKey = "Car_" + currentCarID + "_Spoiler_ID";
            PlayerPrefs.SetInt(saveKey, partToEquip.partID);
        }
        else if (currentCategoryIndex == 1) 
        {
            if (currentCarFrontWheelPoint == null || currentCarRearWheelPoint == null) return;

            currentCarFrontWheelPoint.sprite = partToEquip.sideViewSprite;
            currentCarRearWheelPoint.sprite = partToEquip.sideViewSprite;
            
            string saveKey = "Car_" + currentCarID + "_Wheels_ID";
            PlayerPrefs.SetInt(saveKey, partToEquip.partID);
        }
        else if (currentCategoryIndex == 2)
        {
            if (currentCarBumperPoint == null) return; 

            currentCarBumperPoint.sprite = partToEquip.sideViewSprite;
            string saveKey = "Car_" + currentCarID + "_Bumper_ID";
            PlayerPrefs.SetInt(saveKey, partToEquip.partID);
        }

        PlayerPrefs.Save();
        SelectPart(partToEquip);
    }

    private void LoadTuning()
    {
        // 1. ŁADOWANIE SPOILERA
        if (currentCarSpoilerPoint != null)
        {
            string saveKey = "Car_" + currentCarID + "_Spoiler_ID";
            int savedSpoilerID = PlayerPrefs.GetInt(saveKey, 0);
            bool spoilerFound = false;

            foreach (TuningPart part in allSpoilers)
            {
                if (part != null && part.partID == savedSpoilerID)
                {
                    currentCarSpoilerPoint.sprite = part.sideViewSprite;
                    spoilerFound = true;
                    break;
                }
            }
            if (!spoilerFound || savedSpoilerID == 0) currentCarSpoilerPoint.sprite = null; 
        }

        // 2. ŁADOWANIE ZDERZAKA
        if (currentCarBumperPoint != null)
        {
            string saveKeyBumper = "Car_" + currentCarID + "_Bumper_ID";
            int savedBumperID = PlayerPrefs.GetInt(saveKeyBumper, 0);
            bool bumperFound = false;

            foreach (TuningPart part in allBumpers)
            {
                if (part != null && part.partID == savedBumperID)
                {
                    currentCarBumperPoint.sprite = part.sideViewSprite;
                    bumperFound = true;
                    break;
                }
            }
            if (!bumperFound || savedBumperID == 0) currentCarBumperPoint.sprite = null; 
        }

        // 3. ŁADOWANIE KÓŁ
        if (currentCarFrontWheelPoint != null && currentCarRearWheelPoint != null)
        {
            string saveKeyWheels = "Car_" + currentCarID + "_Wheels_ID";
            int savedWheelsID = PlayerPrefs.GetInt(saveKeyWheels, 0);
            bool wheelsFound = false;

            foreach (TuningPart part in allWheels)
            {
                if (part != null && part.partID == savedWheelsID)
                {
                    currentCarFrontWheelPoint.sprite = part.sideViewSprite;
                    currentCarRearWheelPoint.sprite = part.sideViewSprite;
                    wheelsFound = true;
                    break;
                }
            }
            if (!wheelsFound || savedWheelsID == 0) 
            {
                currentCarFrontWheelPoint.sprite = null; 
                currentCarRearWheelPoint.sprite = null; 
            }
        }
    }

    private void UpdateMoneyUI()
    {
        if (moneyText != null) moneyText.text = PlayerPrefs.GetInt("TotalPoints", 0).ToString();
    }
}