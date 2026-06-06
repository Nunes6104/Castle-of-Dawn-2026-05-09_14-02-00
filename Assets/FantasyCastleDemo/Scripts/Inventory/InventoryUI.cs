using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class InventoryUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private Transform itemListParent;
    [SerializeField] private GameObject itemButtonPrefab;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private GameObject crosshair;

    [Header("Player")]
    [SerializeField] private MonoBehaviour playerController;

    [Header("Text")]
    [SerializeField] private string defaultDescriptionText = "Select an item to inspect it.";

    private bool isOpen = false;

    private void Start()
    {
        inventoryPanel.SetActive(false);
        descriptionText.text = defaultDescriptionText;

        if (GameUIState.Instance != null)
        {
            GameUIState.Instance.SetInventoryOpen(false);
        }

        if (crosshair != null)
        {
            crosshair.SetActive(true);
        }
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame)
        {
            if (GameUIState.Instance != null && GameUIState.Instance.IsCodePanelOpen)
            {
                return;
            }

            ToggleInventory();
        }
    }

    private void ToggleInventory()
    {
        isOpen = !isOpen;
        inventoryPanel.SetActive(isOpen);

        if (GameUIState.Instance != null)
        {
            GameUIState.Instance.SetInventoryOpen(isOpen);
        }

        if (isOpen)
        {
            descriptionText.text = defaultDescriptionText;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (crosshair != null)
            {
                crosshair.SetActive(false);
            }

            if (playerController != null)
            {
                playerController.enabled = false;
            }

            RefreshInventoryUI();
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (crosshair != null)
            {
                crosshair.SetActive(true);
            }

            if (playerController != null)
            {
                playerController.enabled = true;
            }
        }
    }

    private void RefreshInventoryUI()
    {
        foreach (Transform child in itemListParent)
        {
            Destroy(child.gameObject);
        }

        foreach (InventoryItemData item in InventoryManager.Instance.Items)
        {
            GameObject buttonObj = Instantiate(itemButtonPrefab, itemListParent);

            TMP_Text buttonText = buttonObj.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                buttonText.text = item.itemName;
            }

            Button button = buttonObj.GetComponent<Button>();
            if (button != null)
            {
                InventoryItemData selectedItem = item;
                button.onClick.AddListener(() => ShowDescription(selectedItem.description));
            }
        }
    }

    private void ShowDescription(string description)
    {
        descriptionText.text = description;
    }
}