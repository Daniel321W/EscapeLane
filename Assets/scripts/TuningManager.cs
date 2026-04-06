using UnityEngine;
using UnityEngine.UI;

public class TuningManager : MonoBehaviour
{
    [Header("Baza wszystkich części")]
    public TuningPart[] allSpoilers; 
    public TuningPart[] allWheels;      

    [Header("Zakładki (Tabs)")]
    public Button tabAeroButton;        
    public Button tabWheelsButton;      

    [Header("Shop UI Settings")]
    public GameObject shopPanel;        
    public GameObject partButtonPrefab; 
    public Transform scrollContent;     
    public Text moneyText;              

    [Header("Big Action Button")]
    public Button actionBottomButton;   
    public Text actionBottomButtonText; 

    private SpriteRenderer currentCarSpoilerPoint;
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

        // TROP 1: Sprawdzamy, na jakie auto faktycznie przełącza się skrypt
        Debug.LogWarning($"--- ZMIANA AUTA --- Auto ID: {currentCarID} | Nazwa obiektu w grze: {currentCarObject.name} | Czy znaleziono SpoilerPoint?: {currentCarSpoilerPoint != null}");

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

        TuningPart[] activeDatabase = (currentCategoryIndex == 0) ? allSpoilers : allWheels;

        if (currentCategoryIndex == 0 && currentCarSpoilerPoint == null)
        {
            return; 
        }

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
            if (currentCategoryIndex == 0 && currentCarSpoilerPoint == null)
            {
                actionBottomButtonText.text = "STOCK SPOILER (UNMODIFIABLE)";
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
            if (currentCarSpoilerPoint == null) 
            {
                Debug.LogError($"BŁĄD ZAPISU: Próbujesz założyć spoiler, ale Auto ID {currentCarID} nie ma SpoilerPoint!");
                return; 
            }

            currentCarSpoilerPoint.sprite = partToEquip.sideViewSprite;
            string saveKey = "Car_" + currentCarID + "_Spoiler_ID";
            PlayerPrefs.SetInt(saveKey, partToEquip.partID);
            
            // TROP 2: Sprawdzamy co się zapisuje przy kupnie/wybieraniu
            Debug.LogWarning($"[ZAPIS] Zakładam i zapisuję spoiler! Auto ID: {currentCarID} | Otrzymuje Spoiler ID: {partToEquip.partID} ({partToEquip.partName}) | Pod kluczem: {saveKey}");
        }
        else if (currentCategoryIndex == 1) 
        {
            Debug.Log("System nakładania kół jeszcze nie jest zaprogramowany!");
        }

        PlayerPrefs.Save();
    }
    private void LoadTuning()
    {
        if (currentCarSpoilerPoint != null)
        {
            string saveKey = "Car_" + currentCarID + "_Spoiler_ID";
            int savedSpoilerID = PlayerPrefs.GetInt(saveKey, 0);

            // TROP 3: Co skrypt wyciąga z pamięci po przełączeniu strzałką
            Debug.LogWarning($"[WCZYTYWANIE] Auto ID: {currentCarID} | Szukam w pamięci klucza: {saveKey} | Wyciągnięta wartość: {savedSpoilerID}");

            bool spoilerFound = false;

            foreach (TuningPart part in allSpoilers)
            {
                if (part != null && part.partID == savedSpoilerID)
                {
                    currentCarSpoilerPoint.sprite = part.sideViewSprite;
                    spoilerFound = true;
                    Debug.LogWarning($"[WCZYTYWANIE] SUKCES! Baza posiada spoiler o ID {savedSpoilerID}. Zmieniam obrazek na na aucie na ten spoiler.");
                    break;
                }
            }

            if (!spoilerFound || savedSpoilerID == 0)
            {
                currentCarSpoilerPoint.sprite = null; 
                Debug.LogWarning($"[WCZYTYWANIE] PUSTO. Wartość to 0 lub nie ma takiego spoilera w bazie. Zdejmuję stary spoiler (czyszczę obrazek).");
            }
        }
    }

    private void UpdateMoneyUI()
    {
        if (moneyText != null) moneyText.text = PlayerPrefs.GetInt("TotalPoints", 0).ToString();
    }
}