using UnityEngine;

public class CodeChest : MonoBehaviour, IInteractable
{
    [Header("Chest Objects")]
    [SerializeField] private GameObject closedChest;
    [SerializeField] private GameObject openChest;

    [Header("Code Settings")]
    [SerializeField] private string correctCode = "5937";

    [Header("Reward Object")]
    [SerializeField] private GameObject rewardObject;

    [Header("Interaction Collider")]
    [SerializeField] private Collider interactionCollider;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip successSound;
    [SerializeField] private AudioClip failureSound;

    private bool isOpen = false;

    public string InteractionText => isOpen ? "" : "Press E to enter code";

    private void Start()
    {
        if (closedChest != null)
        {
            closedChest.SetActive(true);
        }

        if (openChest != null)
        {
            openChest.SetActive(false);
        }

        if (rewardObject != null)
        {
            rewardObject.SetActive(false);
        }

        if (interactionCollider != null)
        {
            interactionCollider.enabled = true;
        }
    }

    public void Interact()
    {
        if (isOpen)
        {
            return;
        }

        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.OnCodeChestInteractedWithoutCode();
        }

        if (CodeChestUI.Instance != null)
        {
            CodeChestUI.Instance.OpenPanel(this);
        }
        else
        {
            Debug.LogWarning("CodeChestUI was not found in the scene.");
        }
    }

    public bool TryOpenWithCode(string enteredCode)
    {
        if (isOpen)
        {
            return false;
        }

        if (enteredCode == correctCode)
        {
            OpenChest();
            return true;
        }

        PlayFailureSound();
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

        if (rewardObject != null)
        {
            rewardObject.SetActive(true);
        }

        if (interactionCollider != null)
        {
            interactionCollider.enabled = false;
        }

        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.OnCodeChestOpened();
        }

        PlaySuccessSound();

        Debug.Log("Chest opened. Reward object is now available.");
    }

    private void PlaySuccessSound()
    {
        if (audioSource != null && successSound != null)
        {
            audioSource.PlayOneShot(successSound);
        }
    }

    private void PlayFailureSound()
    {
        if (audioSource != null && failureSound != null)
        {
            audioSource.PlayOneShot(failureSound);
        }
    }
}