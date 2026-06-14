using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    public string itemName;
    public string itemDescription;
    public Sprite itemIcon;

    public InventoryItem(string itemName, string itemDescription, Sprite itemIcon)
    {
        this.itemName = itemName;
        this.itemDescription = itemDescription;
        this.itemIcon = itemIcon;
    }
}

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    private readonly List<InventoryItem> items = new List<InventoryItem>();

    public IReadOnlyList<InventoryItem> Items => items;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void AddItem(string itemName, string itemDescription, Sprite itemIcon = null)
    {
        if (HasItem(itemName))
        {
            Debug.Log("Item already exists in inventory: " + itemName);
            return;
        }

        InventoryItem newItem = new InventoryItem(itemName, itemDescription, itemIcon);
        items.Add(newItem);

        Debug.Log("Added to inventory: " + itemName);

        RefreshInventoryUI();
    }

    public bool RemoveItem(string itemName)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].itemName == itemName)
            {
                items.RemoveAt(i);

                Debug.Log("Removed from inventory: " + itemName);

                RefreshInventoryUI();

                return true;
            }
        }

        Debug.LogWarning("Could not remove item because it was not found: " + itemName);
        return false;
    }

    public bool HasItem(string itemName)
    {
        foreach (InventoryItem item in items)
        {
            if (item.itemName == itemName)
            {
                return true;
            }
        }

        return false;
    }

    public InventoryItem GetItem(string itemName)
    {
        foreach (InventoryItem item in items)
        {
            if (item.itemName == itemName)
            {
                return item;
            }
        }

        return null;
    }

    private void RefreshInventoryUI()
    {
        InventoryUI inventoryUI = FindObjectOfType<InventoryUI>();

        if (inventoryUI != null)
        {
            inventoryUI.RefreshInventory();
        }
    }
}