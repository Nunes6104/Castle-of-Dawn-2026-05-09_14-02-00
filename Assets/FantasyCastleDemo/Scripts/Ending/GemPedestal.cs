using UnityEngine;
using UnityEngine.UI;

public class GemPedestal : MonoBehaviour, IInteractable
{
    [Header("Required Gem")]
    [SerializeField] private string requiredItemName = "Strange Gem";

    [Header("Pedestal Visuals")]
    [SerializeField] private GameObject placedGemVisual;
    [SerializeField] private Image gemIconImage;
    [SerializeField] private Sprite gemIconSprite;

    [Header("Final Door")]
    [SerializeField] private FinalDoorController finalDoorController;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip placeGemSound;
    [SerializeField] private AudioClip missingGemSound;

    [Header("Interaction Text")]
    [SerializeField] private string placeTextPrefix = "Press E to place ";
    [SerializeField] private string placedText = "Gem already placed";
    [SerializeField] private string missingGemText = "You do not have this gem";

    private bool isPlaced = false;
    private string temporaryInteractionMessage = "";
    private float temporaryMessageEndTime = 0f;

    public bool IsPlaced => isPlaced;
    public string RequiredItemName => requiredItemName;

    public string InteractionText
    {
        get
        {
            if (!string.IsNullOrEmpty(temporaryInteractionMessage) && Time.time < temporaryMessageEndTime)
            {
                return temporaryInteractionMessage;
            }

            if (isPlaced)
            {
                return placedText;
            }

            return placeTextPrefix + requiredItemName;
        }
    }

    private void Start()
    {
        if (placedGemVisual != null)
        {
            placedGemVisual.SetActive(false);
        }

        if (gemIconImage != null)
        {
            gemIconImage.sprite = gemIconSprite;
            gemIconImage.enabled = gemIconSprite != null;
        }
    }

    public void Interact()
    {
        if (isPlaced)
        {
            ShowTemporaryMessage(placedText, 1.5f);
            return;
        }

        if (InventoryManager.Instance == null)
        {
            Debug.LogWarning("InventoryManager was not found in the scene.");
            return;
        }

        if (!InventoryManager.Instance.HasItem(requiredItemName))
        {
            ShowTemporaryMessage(missingGemText, 2f);
            PlayMissingGemSound();

            Debug.Log("Player does not have required gem: " + requiredItemName);
            return;
        }

        bool removed = InventoryManager.Instance.RemoveItem(requiredItemName);

        if (!removed)
        {
            ShowTemporaryMessage(missingGemText, 2f);
            return;
        }

        PlaceGem();
    }

    private void PlaceGem()
    {
        isPlaced = true;

        if (placedGemVisual != null)
        {
            placedGemVisual.SetActive(true);
        }

        PlayPlaceGemSound();

        if (finalDoorController != null)
        {
            finalDoorController.RegisterGemPlaced(this);
        }

        Debug.Log("Placed gem on pedestal: " + requiredItemName);
    }

    private void ShowTemporaryMessage(string message, float duration)
    {
        temporaryInteractionMessage = message;
        temporaryMessageEndTime = Time.time + duration;
    }

    private void PlayPlaceGemSound()
    {
        if (audioSource != null && placeGemSound != null)
        {
            audioSource.PlayOneShot(placeGemSound);
        }
    }

    private void PlayMissingGemSound()
    {
        if (audioSource != null && missingGemSound != null)
        {
            audioSource.PlayOneShot(missingGemSound);
        }
    }
}