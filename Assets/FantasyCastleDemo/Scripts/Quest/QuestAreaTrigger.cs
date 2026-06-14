using UnityEngine;

public class QuestAreaTrigger : MonoBehaviour
{
    private enum QuestAreaType
    {
        Smithy,
        ThroneRoom
    }

    [Header("Quest Area")]
    [SerializeField] private QuestAreaType questAreaType = QuestAreaType.Smithy;

    [Header("Trigger Settings")]
    [SerializeField] private bool triggerOnlyOnce = true;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggerOnlyOnce && hasTriggered)
        {
            return;
        }

        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (QuestManager.Instance == null)
        {
            Debug.LogWarning("QuestManager was not found in the scene.");
            return;
        }

        hasTriggered = true;

        switch (questAreaType)
        {
            case QuestAreaType.Smithy:
                QuestManager.Instance.OnSmithyEntered();
                Debug.Log("Quest updated: Smithy entered.");
                break;

            case QuestAreaType.ThroneRoom:
                QuestManager.Instance.OnThroneRoomEntered();
                Debug.Log("Quest updated: Throne room entered.");
                break;
        }
    }
}