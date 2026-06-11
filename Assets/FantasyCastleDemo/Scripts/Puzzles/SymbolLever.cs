using System.Collections;
using UnityEngine;

public class SymbolLever : MonoBehaviour, IInteractable
{
    [Header("Lever Symbol")]
    [SerializeField] private string symbolId = "Moon";

    [Header("Linked Puzzle Chest")]
    [SerializeField] private SymbolLeverChest linkedChest;

    [Header("Lever Visual")]
    [SerializeField] private Transform leverHandle;

    [Tooltip("Rotation when the lever is pulled/down.")]
    [SerializeField] private Vector3 activatedLocalEulerRotation = new Vector3(-45f, 0f, 0f);

    [SerializeField] private float rotationSpeed = 6f;

    [Header("Interaction Text")]
    [SerializeField] private string interactionText = "Press E to pull lever";
    [SerializeField] private string alreadyPulledText = "Lever already pulled";

    private Quaternion defaultRotation;
    private Quaternion activatedRotation;
    private Coroutine rotationCoroutine;
    private bool isActivated = false;

    public string SymbolId => symbolId;
    public bool IsActivated => isActivated;
    public string InteractionText => isActivated ? alreadyPulledText : interactionText;

    private void Awake()
    {
        if (leverHandle == null)
        {
            leverHandle = transform;
        }

        defaultRotation = leverHandle.localRotation;
        activatedRotation = Quaternion.Euler(activatedLocalEulerRotation);
    }

    public void Interact()
    {
        if (isActivated)
        {
            return;
        }

        ActivateLever();
    }

    private void ActivateLever()
    {
        isActivated = true;

        RotateLeverTo(activatedRotation);

        if (linkedChest != null)
        {
            linkedChest.RegisterLever(symbolId);
        }
        else
        {
            Debug.LogWarning("Lever " + gameObject.name + " has no linked chest.");
        }

        Debug.Log("Lever pulled: " + symbolId);
    }

    public void ResetLever()
    {
        isActivated = false;
        RotateLeverTo(defaultRotation);

        Debug.Log("Lever reset: " + symbolId);
    }

    private void RotateLeverTo(Quaternion targetRotation)
    {
        if (leverHandle == null)
        {
            return;
        }

        if (rotationCoroutine != null)
        {
            StopCoroutine(rotationCoroutine);
        }

        rotationCoroutine = StartCoroutine(RotateLeverCoroutine(targetRotation));
    }

    private IEnumerator RotateLeverCoroutine(Quaternion targetRotation)
    {
        while (Quaternion.Angle(leverHandle.localRotation, targetRotation) > 0.1f)
        {
            leverHandle.localRotation = Quaternion.Lerp(
                leverHandle.localRotation,
                targetRotation,
                Time.deltaTime * rotationSpeed
            );

            yield return null;
        }

        leverHandle.localRotation = targetRotation;
    }
}