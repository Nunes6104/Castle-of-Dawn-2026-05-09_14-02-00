using UnityEngine;

public class CodeChest : MonoBehaviour, IInteractable
{
    [Header("Chest Objects")]
    [SerializeField] private GameObject closedChest;
    [SerializeField] private GameObject openChest;

    [Header("Code Settings")]
    [SerializeField] private string requiredItem = "PaperCode";
    [SerializeField] private string correctCode = "5937";

    [Header("Reward")]
    [SerializeField] private string rewardItem = "ItemA";

    [TextArea]
    [SerializeField] private string rewardDescription = "A mysterious item found inside the chest.";

    private bool isOpen = false;

    public string InteractionText => isOpen ? "Chest already opened" : "Press E to enter code";

    public void Interact()
    {
        if (isOpen)
        {
            UnityEngine.Debug.Log("This chest is already open.");
            return;
        }

        if (!InventoryManager.Instance.HasItem(requiredItem))
        {
            UnityEngine.Debug.Log("You need to find the paper with the code first.");
            return;
        }

        CodeChestUI.Instance.OpenPanel(this);
    }

    public bool TryOpenWithCode(string enteredCode)
    {
        if (enteredCode == correctCode)
        {
            OpenChest();
            return true;
        }

        return false;
    }

    private void OpenChest()
    {
        isOpen = true;

        if (closedChest != null)
        {
            closedChest.SetActive(false);
        }

        if (openChest != null)
        {
            openChest.SetActive(true);
        }

        InventoryManager.Instance.AddItem(rewardItem, rewardDescription);

        UnityEngine.Debug.Log("Chest opened. You got: " + rewardItem);
    }
}