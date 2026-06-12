using System.Collections.Generic;
using UnityEngine;

public class StatuePuzzleChest : MonoBehaviour, IInteractable
{
    [Header("Chest Objects")]
    [SerializeField] private GameObject closedChest;
    [SerializeField] private GameObject openChest;

    [Header("Statue Puzzle")]
    [SerializeField] private List<RotatingStatue> statues = new List<RotatingStatue>();

    [Header("Reward Object")]
    [SerializeField] private GameObject rewardObject;

    [Header("Interaction Collider")]
    [SerializeField] private Collider interactionCollider;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip successSound;
    [SerializeField] private AudioClip failureSound;

    [Header("Interaction Text")]
    [SerializeField] private string interactionText = "Press E to check the statues";

    private bool isOpen = false;

    public string InteractionText => isOpen ? "" : interactionText;

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

        if (AreAllStatuesCorrect())
        {
            OpenChest();
        }
        else
        {
            ResetAllStatues();
            PlayFailureSound();
            Debug.Log("Statue puzzle is incorrect. All statues were reset.");
        }
    }

    private bool AreAllStatuesCorrect()
    {
        foreach (RotatingStatue statue in statues)
        {
            if (statue == null)
            {
                Debug.LogWarning("A statue reference is missing in StatuePuzzleChest.");
                return false;
            }

            if (!statue.IsCorrectlyRotated())
            {
                Debug.Log(
                    statue.StatueName +
                    " is not correctly rotated. Current Y: " +
                    Mathf.Round(statue.transform.localEulerAngles.y) +
                    " | Correct Y: " +
                    statue.CorrectYRotation
                );

                return false;
            }
        }

        return true;
    }

    private void ResetAllStatues()
    {
        foreach (RotatingStatue statue in statues)
        {
            if (statue != null)
            {
                statue.ResetStatue();
            }
        }
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

        PlaySuccessSound();

        Debug.Log("Statue puzzle solved. Chest opened.");
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