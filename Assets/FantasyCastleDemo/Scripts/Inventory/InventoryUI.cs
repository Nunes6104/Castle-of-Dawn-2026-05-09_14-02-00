using TMPro;
using UnityEngine;
using UnityEngine.UI;
using StarterAssets;

public class InventoryUI : MonoBehaviour
{
    [Header("Inventory UI")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private Transform itemListParent;
    [SerializeField] private GameObject itemButtonPrefab;

    [Header("Item Details UI")]
    [SerializeField] private Image selectedItemImage;
    [SerializeField] private TMP_Text selectedItemTitleText;
    [SerializeField] private TMP_Text selectedItemDescriptionText;

    [Header("Default Icon")]
    [SerializeField] private Sprite defaultItemIcon;

    private bool isOpen = false;
    private StarterAssetsInputs starterAssetsInputs;

    private void Start()
    {
        starterAssetsInputs = FindObjectOfType<StarterAssetsInputs>();

        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }

        SetCursorAndCameraState(false);
        ClearItemDetails();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        isOpen = !isOpen;

        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(isOpen);
        }

        SetCursorAndCameraState(isOpen);

        if (isOpen)
        {
            RefreshInventory();
        }
        else
        {
            ClearItemDetails();
        }
    }

    private void SetCursorAndCameraState(bool inventoryOpen)
    {
        if (inventoryOpen)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            if (starterAssetsInputs != null)
            {
                starterAssetsInputs.cursorInputForLook = false;
                starterAssetsInputs.look = Vector2.zero;
            }
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            if (starterAssetsInputs != null)
            {
                starterAssetsInputs.cursorInputForLook = true;
                starterAssetsInputs.look = Vector2.zero;
            }
        }
    }

    public void RefreshInventory()
    {
        if (itemListParent == null || itemButtonPrefab == null)
        {
            Debug.LogWarning("InventoryUI is missing itemListParent or itemButtonPrefab.");
            return;
        }

        foreach (Transform child in itemListParent)
        {
            Destroy(child.gameObject);
        }

        if (InventoryManager.Instance == null)
        {
            return;
        }

        foreach (InventoryItem item in InventoryManager.Instance.Items)
        {
            GameObject newButtonObject = Instantiate(itemButtonPrefab, itemListParent);

            TMP_Text buttonText = newButtonObject.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                buttonText.text = item.itemName;
            }

            Image[] images = newButtonObject.GetComponentsInChildren<Image>();
            foreach (Image image in images)
            {
                if (image.gameObject != newButtonObject)
                {
                    image.sprite = item.itemIcon != null ? item.itemIcon : defaultItemIcon;
                    image.enabled = image.sprite != null;
                    break;
                }
            }

            Button button = newButtonObject.GetComponent<Button>();
            if (button != null)
            {
                InventoryItem capturedItem = item;
                button.onClick.AddListener(() => ShowItemDetails(capturedItem));
            }
        }
    }

    private void ShowItemDetails(InventoryItem item)
    {
        if (item == null)
        {
            ClearItemDetails();
            return;
        }

        if (selectedItemTitleText != null)
        {
            selectedItemTitleText.text = item.itemName;
        }

        if (selectedItemDescriptionText != null)
        {
            selectedItemDescriptionText.text = item.itemDescription;
        }

        if (selectedItemImage != null)
        {
            selectedItemImage.sprite = item.itemIcon != null ? item.itemIcon : defaultItemIcon;
            selectedItemImage.enabled = selectedItemImage.sprite != null;
        }
    }

    private void ClearItemDetails()
    {
        if (selectedItemTitleText != null)
        {
            selectedItemTitleText.text = "";
        }

        if (selectedItemDescriptionText != null)
        {
            selectedItemDescriptionText.text = "";
        }

        if (selectedItemImage != null)
        {
            selectedItemImage.sprite = null;
            selectedItemImage.enabled = false;
        }
    }
}