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

        // UWAGA: Szukamy punktów o nazwach "FrontWheelsPoint" i "RearWheelsPoint" - upewnij się, że tak się nazywają w Unity!
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
            // Zaktualizowano o blokadę dla kół, gdyby auto ich nie miało
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
                actionBottomButtonText.text = "SELECT";
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

        if (isPartOwned)
        {
            actionBottomButtonText.text = "EQUIP " + clickedPart.partName;
        }
        else
        {
            actionBottomButtonText.text = "BUY " + clickedPart.partName + " ($" + clickedPart.price + ")";
        }
    }

    public void OnMainActionClicked()
    {
        if (currentlySelectedPart == null) return; 

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

    private void EquipPart(TuningPart partToEquip)
    {
        if (currentCategoryIndex == 0) 
        {
            if (currentCarSpoilerPoint == null) return; 

            currentCarSpoilerPoint.sprite = partToEquip.sideViewSprite;
            string saveKey = "Car_" + currentCarID + "_Spoiler_ID";
            PlayerPrefs.SetInt(saveKey, partToEquip.partID);
        }
        // DODANO: Zakładanie Kół (na przód i tył jednocześnie)
        else if (currentCategoryIndex == 1) 
        {
            if (currentCarFrontWheelPoint == null || currentCarRearWheelPoint == null) return;

            currentCarFrontWheelPoint.sprite = partToEquip.sideViewSprite;
            currentCarRearWheelPoint.sprite = partToEquip.sideViewSprite;
            
            string saveKey = "Car_" + currentCarID + "_Wheels_ID";
            PlayerPrefs.SetInt(saveKey, partToEquip.partID);
            
            Debug.LogWarning($"[ZAPIS] Koła zmienione! Auto: {currentCarID} | Koła ID: {partToEquip.partID} | Klucz: {saveKey}");
        }
        else if (currentCategoryIndex == 2)
        {
            if (currentCarBumperPoint == null) return; 

            currentCarBumperPoint.sprite = partToEquip.sideViewSprite;
            string saveKey = "Car_" + currentCarID + "_Bumper_ID";
            PlayerPrefs.SetInt(saveKey, partToEquip.partID);
            
            Debug.LogWarning($"[ZAPIS] Zderzak zmieniony! Auto: {currentCarID} | Zderzak ID: {partToEquip.partID} | Klucz: {saveKey}");
        }

        PlayerPrefs.Save();
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

            if (!spoilerFound || savedSpoilerID == 0)
            {
                currentCarSpoilerPoint.sprite = null; 
            }
        } // <--- Tutaj kończy się sprawdzanie spoilera!

        // 2. ŁADOWANIE ZDERZAKA (Wyciągnięte na zewnątrz!)
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

        // 3. DODANO: ŁADOWANIE KÓŁ
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
                    Debug.LogWarning($"[WCZYTYWANIE] Koła znane! ID: {savedWheelsID} na aucie {currentCarID}.");
                    break;
                }
            }
            if (!wheelsFound || savedWheelsID == 0) 
            {
                // Jeśli nie ma kupionych kół, ściągamy obrazek (powinny być widoczne bazowe koła z rysunku auta)
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