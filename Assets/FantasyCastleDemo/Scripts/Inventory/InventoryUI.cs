using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

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

    [Header("Scripts to disable while inventory is open")]
    [SerializeField] private Behaviour[] scriptsToDisableWhileOpen;

    private bool isOpen = false;

    private void Start()
    {
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }

        ClearItemDetails();

        if (GameUIState.Instance != null)
        {
            GameUIState.Instance.SetInventoryOpen(false);
        }
    }

    private void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (GameUIState.Instance != null)
        {
            if (GameUIState.Instance.IsEndingOpen)
            {
                if (isOpen)
                {
                    CloseInventory();
                }

                return;
            }

            if (!isOpen && GameUIState.Instance.IsCodePanelOpen)
            {
                return;
            }

            if (!isOpen && GameUIState.Instance.IsPauseMenuOpen)
            {
                return;
            }
        }

        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        if (isOpen)
        {
            CloseInventory();
        }
        else
        {
            OpenInventory();
        }
    }

    private void OpenInventory()
    {
        if (GameUIState.Instance != null)
        {
            if (GameUIState.Instance.IsEndingOpen ||
                GameUIState.Instance.IsCodePanelOpen ||
                GameUIState.Instance.IsPauseMenuOpen)
            {
                return;
            }
        }

        isOpen = true;

        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(true);
        }

        if (GameUIState.Instance != null)
        {
            GameUIState.Instance.SetInventoryOpen(true);
        }

        SetPlayerScripts(false);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.OnInventoryOpened();
        }

        RefreshInventory();
    }

    private void CloseInventory()
    {
        isOpen = false;

        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }

        if (GameUIState.Instance != null)
        {
            GameUIState.Instance.SetInventoryOpen(false);
        }

        ClearItemDetails();

        SetPlayerScripts(true);

        if (GameUIState.Instance == null || !GameUIState.Instance.IsAnyUIOpen)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
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

    private void SetPlayerScripts(bool enabled)
    {
        foreach (Behaviour script in scriptsToDisableWhileOpen)
        {
            if (script != null)
            {
                script.enabled = enabled;
            }
        }
    }
}