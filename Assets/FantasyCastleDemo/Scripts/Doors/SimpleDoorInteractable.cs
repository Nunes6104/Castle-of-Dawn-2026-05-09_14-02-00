using System.Collections;
using UnityEngine;

public class SimpleDoorInteractable : MonoBehaviour, IInteractable
{
    [Header("Door Settings")]
    [SerializeField] private Transform doorPivot;
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float openSpeed = 3f;
    [SerializeField] private bool isOpen = false;

    [Header("Interaction Text")]
    [SerializeField] private string openText = "Press E to open door";
    [SerializeField] private string closeText = "Press E to close door";

    private Quaternion closedRotation;
    private Quaternion openedRotation;
    private Coroutine currentCoroutine;

    public string InteractionText => isOpen ? closeText : openText;

    private void Start()
    {
        if (doorPivot == null)
        {
            doorPivot = transform;
        }

        closedRotation = doorPivot.localRotation;
        openedRotation = closedRotation * Quaternion.Euler(0f, openAngle, 0f);
    }

    public void Interact()
    {
        isOpen = !isOpen;

        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        currentCoroutine = StartCoroutine(RotateDoor());
    }

    private IEnumerator RotateDoor()
    {
        Quaternion targetRotation = isOpen ? openedRotation : closedRotation;

        while (Quaternion.Angle(doorPivot.localRotation, targetRotation) > 0.1f)
        {
            doorPivot.localRotation = Quaternion.Lerp(
                doorPivot.localRotation,
                targetRotation,
                Time.deltaTime * openSpeed
            );

            yield return null;
        }

        doorPivot.localRotation = targetRotation;
    }
}