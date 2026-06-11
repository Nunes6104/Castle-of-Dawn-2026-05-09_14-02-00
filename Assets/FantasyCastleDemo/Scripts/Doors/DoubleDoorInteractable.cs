using System.Collections;
using UnityEngine;

public class DoubleDoorInteractable : MonoBehaviour, IInteractable
{
    [Header("Door Leaves")]
    [SerializeField] private Transform leftDoor;
    [SerializeField] private Transform rightDoor;

    [Header("Door Settings")]
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float openSpeed = 3f;
    [SerializeField] private bool isOpen = false;

    [Header("Interaction Text")]
    [SerializeField] private string openText = "Press E to open doors";
    [SerializeField] private string closeText = "Press E to close doors";

    private Quaternion leftClosedRotation;
    private Quaternion rightClosedRotation;

    private Quaternion leftOpenedRotation;
    private Quaternion rightOpenedRotation;

    private Coroutine currentCoroutine;

    public string InteractionText => isOpen ? closeText : openText;

    private void Start()
    {
        if (leftDoor == null || rightDoor == null)
        {
            Debug.LogWarning("DoubleDoorInteractable is missing leftDoor or rightDoor.");
            return;
        }

        leftClosedRotation = leftDoor.localRotation;
        rightClosedRotation = rightDoor.localRotation;

        leftOpenedRotation = leftClosedRotation * Quaternion.Euler(0f, -openAngle, 0f);
        rightOpenedRotation = rightClosedRotation * Quaternion.Euler(0f, openAngle, 0f);
    }

    public void Interact()
    {
        if (leftDoor == null || rightDoor == null)
        {
            return;
        }

        isOpen = !isOpen;

        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        currentCoroutine = StartCoroutine(RotateDoors());
    }

    private IEnumerator RotateDoors()
    {
        Quaternion targetLeftRotation = isOpen ? leftOpenedRotation : leftClosedRotation;
        Quaternion targetRightRotation = isOpen ? rightOpenedRotation : rightClosedRotation;

        while (
            Quaternion.Angle(leftDoor.localRotation, targetLeftRotation) > 0.1f ||
            Quaternion.Angle(rightDoor.localRotation, targetRightRotation) > 0.1f
        )
        {
            leftDoor.localRotation = Quaternion.Lerp(
                leftDoor.localRotation,
                targetLeftRotation,
                Time.deltaTime * openSpeed
            );

            rightDoor.localRotation = Quaternion.Lerp(
                rightDoor.localRotation,
                targetRightRotation,
                Time.deltaTime * openSpeed
            );

            yield return null;
        }

        leftDoor.localRotation = targetLeftRotation;
        rightDoor.localRotation = targetRightRotation;
    }
}