using UnityEngine;

public class PickupItem : MonoBehaviour, IInteractable
{
    [Header("Item Settings")]
    [SerializeField] private string itemName = "PaperCode";

    [TextArea]
    [SerializeField] private string itemDescription = "A small paper with a handwritten code: 5937";

    public string InteractionText => "Press E to pick up " + itemName;

    public void Interact()
    {
        InventoryManager.Instance.AddItem(itemName, itemDescription);

        UnityEngine.Debug.Log(itemDescription);

        Destroy(gameObject);
    }
}