using UnityEngine;

public class PickupItem : MonoBehaviour, IInteractable
{
    [Header("Item Data")]
    [SerializeField] private string itemName = "ItemA";

    [TextArea]
    [SerializeField] private string itemDescription = "First gem found inside the chest. One of three items needed to open the final door.";

    [SerializeField] private Sprite itemIcon;

    [Header("Interaction Text")]
    [SerializeField] private string interactionText = "Press E to pick up item";

    public string InteractionText => interactionText;

    public void Interact()
    {
        if (InventoryManager.Instance == null)
        {
            Debug.LogWarning("InventoryManager was not found in the scene.");
            return;
        }

        InventoryManager.Instance.AddItem(itemName, itemDescription, itemIcon);

        Debug.Log("Picked up item: " + itemName);

        gameObject.SetActive(false);
    }
}