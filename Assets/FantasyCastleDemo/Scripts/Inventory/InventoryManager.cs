using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryItemData
{
    public string itemName;
    public string description;

    public InventoryItemData(string name, string desc)
    {
        itemName = name;
        description = desc;
    }
}

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    private List<InventoryItemData> items = new List<InventoryItemData>();

    public List<InventoryItemData> Items => items;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void AddItem(string itemName, string description)
    {
        foreach (InventoryItemData item in items)
        {
            if (item.itemName == itemName)
            {
                UnityEngine.Debug.Log("Item already exists: " + itemName);
                return;
            }
        }

        items.Add(new InventoryItemData(itemName, description));
        UnityEngine.Debug.Log("Item added: " + itemName);
    }

    public bool HasItem(string itemName)
    {
        foreach (InventoryItemData item in items)
        {
            if (item.itemName == itemName)
            {
                return true;
            }
        }

        return false;
    }

    public InventoryItemData GetItem(string itemName)
    {
        foreach (InventoryItemData item in items)
        {
            if (item.itemName == itemName)
            {
                return item;
            }
        }

        return null;
    }

    public bool HasAllFinalItems()
    {
        return HasItem("ItemA") && HasItem("ItemB") && HasItem("ItemC");
    }
}