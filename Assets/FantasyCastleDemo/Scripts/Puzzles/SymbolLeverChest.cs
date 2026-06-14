using System.Collections.Generic;
using UnityEngine;

public class SymbolLeverChest : MonoBehaviour, IInteractable
{
    [Header("Chest Objects")]
    [SerializeField] private GameObject closedChest;
    [SerializeField] private GameObject openChest;

    [Header("Lever Puzzle")]
    [SerializeField]
    private List<string> correctOrder = new List<string>
    {
        "Moon",
        "Sun",
        "Star"
    };

    [SerializeField] private List<SymbolLever> levers = new List<SymbolLever>();

    [Header("Reward Object")]
    [SerializeField] private GameObject rewardObject;

    [Header("Interaction Collider")]
    [SerializeField] private Collider interactionCollider;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip wrongSound;
    [SerializeField] private AudioClip correctSound;

    private readonly List<string> currentOrder = new List<string>();
    private bool sequenceHasMistake = false;
    private bool isOpen = false;

    public string InteractionText => isOpen ? "" : "Press E to check the symbols";

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

        ResetLeverStatesOnly();
    }

    public void Interact()
    {
        if (isOpen)
        {
            return;
        }

        if (IsSequenceCorrect())
        {
            OpenChest();
        }
        else
        {
            ResetPuzzleWithWrongFeedback();
        }
    }

    public void RegisterLever(string symbolId)
    {
        if (isOpen)
        {
            return;
        }

        currentOrder.Add(symbolId);

        int currentIndex = currentOrder.Count - 1;

        if (currentIndex >= correctOrder.Count)
        {
            sequenceHasMistake = true;
            Debug.Log("Too many levers pulled.");
            return;
        }

        if (currentOrder[currentIndex] != correctOrder[currentIndex])
        {
            sequenceHasMistake = true;
        }

        Debug.Log("Current lever sequence: " + string.Join(" -> ", currentOrder));
    }

    private bool IsSequenceCorrect()
    {
        if (sequenceHasMistake)
        {
            return false;
        }

        if (currentOrder.Count != correctOrder.Count)
        {
            return false;
        }

        for (int i = 0; i < correctOrder.Count; i++)
        {
            if (currentOrder[i] != correctOrder[i])
            {
                return false;
            }
        }

        return true;
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
            QuestManager.Instance.OnLeverChestOpened();
        }

        PlayCorrectSound();

        Debug.Log("Symbol lever chest opened.");
    }

    private void ResetPuzzleWithWrongFeedback()
    {
        currentOrder.Clear();
        sequenceHasMistake = false;

        foreach (SymbolLever lever in levers)
        {
            if (lever != null)
            {
                lever.ResetLever();
            }
        }

        PlayWrongSound();

        Debug.Log("Wrong lever sequence. Puzzle reset.");
    }

    private void ResetLeverStatesOnly()
    {
        currentOrder.Clear();
        sequenceHasMistake = false;

        foreach (SymbolLever lever in levers)
        {
            if (lever != null)
            {
                lever.ResetLever();
            }
        }
    }

    private void PlayWrongSound()
    {
        if (audioSource != null && wrongSound != null)
        {
            audioSource.PlayOneShot(wrongSound);
        }
    }

    private void PlayCorrectSound()
    {
        if (audioSource != null && correctSound != null)
        {
            audioSource.PlayOneShot(correctSound);
        }
    }
}