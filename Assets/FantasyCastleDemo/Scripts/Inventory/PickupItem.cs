using UnityEngine;

public class PickupItem : MonoBehaviour, IInteractable
{
    private enum QuestPickupType
    {
        None,
        CodePaper,
        BarracksReward,
        SmithyReward,
        ThroneRoomReward
    }

    [Header("Item Data")]
    [SerializeField] private string itemName = "ItemA";

    [TextArea]
    [SerializeField] private string itemDescription = "First gem found inside the chest. One of three items needed to open the final door.";

    [SerializeField] private Sprite itemIcon;

    [Header("Quest")]
    [SerializeField] private QuestPickupType questPickupType = QuestPickupType.None;

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

        UpdateQuestProgress();

        Debug.Log("Picked up item: " + itemName);

        gameObject.SetActive(false);
    }

    private void UpdateQuestProgress()
    {
        if (QuestManager.Instance == null)
        {
            return;
        }

        switch (questPickupType)
        {
            case QuestPickupType.CodePaper:
                QuestManager.Instance.OnCodePaperPickedUp();
                break;

            case QuestPickupType.BarracksReward:
                QuestManager.Instance.OnBarracksRewardCollected();
                break;

            case QuestPickupType.SmithyReward:
                QuestManager.Instance.OnSmithyRewardCollected();
                break;

            case QuestPickupType.ThroneRoomReward:
                QuestManager.Instance.OnThroneRoomRewardCollected();
                break;
        }
    }
}