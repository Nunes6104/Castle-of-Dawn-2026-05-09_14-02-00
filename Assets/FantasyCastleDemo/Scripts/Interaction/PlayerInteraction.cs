using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float interactionDistance = 5f;
    [SerializeField] private Camera playerCamera;

    [Header("UI")]
    [SerializeField] private GameObject interactionPromptObject;
    [SerializeField] private TMP_Text interactionPromptText;
    [SerializeField] private string defaultPromptMessage = "Pressione E para interagir";

    private IInteractable currentInteractable;

    private void Start()
    {
        HidePrompt();
    }

    private void Update()
    {
        if (GameUIState.Instance != null && GameUIState.Instance.IsAnyUIOpen)
        {
            currentInteractable = null;
            HidePrompt();
            return;
        }

        CheckForInteractable();

        if (currentInteractable != null && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            HidePrompt();
            currentInteractable.Interact();
        }
    }

    private void CheckForInteractable()
    {
        currentInteractable = null;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance))
        {
            currentInteractable = hit.collider.GetComponentInParent<IInteractable>();

            if (currentInteractable != null)
            {
                ShowPrompt();
                return;
            }
        }

        HidePrompt();
    }

    private void ShowPrompt()
    {
        if (interactionPromptObject != null)
        {
            interactionPromptObject.SetActive(true);
        }

        if (interactionPromptText != null)
        {
            interactionPromptText.text = defaultPromptMessage;
        }
    }

    private void HidePrompt()
    {
        if (interactionPromptObject != null)
        {
            interactionPromptObject.SetActive(false);
        }
    }
}