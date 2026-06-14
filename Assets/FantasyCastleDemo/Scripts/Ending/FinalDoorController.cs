using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalDoorController : MonoBehaviour, IInteractable
{
    [Header("Required Pedestals")]
    [SerializeField] private int requiredGemCount = 3;

    [Header("Single Door Settings")]
    [SerializeField] private Transform doorPivot;
    [SerializeField] private float openAngle = 90f;

    [Header("Double Door Settings")]
    [SerializeField] private bool useDoubleDoor = true;
    [SerializeField] private Transform leftDoorPivot;
    [SerializeField] private Transform rightDoorPivot;
    [SerializeField] private float leftDoorOpenAngle = -90f;
    [SerializeField] private float rightDoorOpenAngle = 90f;

    [Header("Opening Settings")]
    [SerializeField] private float openSpeed = 3f;

    [Header("Door Collider")]
    [SerializeField] private Collider interactionCollider;

    [Header("Ending")]
    [SerializeField] private DemoEndingUI demoEndingUI;
    [SerializeField] private float endingDelayAfterDoorOpens = 1.5f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip unlockedSound;
    [SerializeField] private AudioClip doorOpenSound;
    [SerializeField] private AudioClip lockedSound;

    [Header("Interaction Text")]
    [SerializeField] private string lockedText = "The door is sealed. Place the three gems.";
    [SerializeField] private string unlockedText = "Press E to open the final door";
    [SerializeField] private string openedText = "";

    private readonly List<GemPedestal> placedPedestals = new List<GemPedestal>();

    private bool isUnlocked = false;
    private bool isOpen = false;
    private bool endingStarted = false;

    private Quaternion singleDoorClosedRotation;
    private Quaternion singleDoorOpenedRotation;

    private Quaternion leftDoorClosedRotation;
    private Quaternion leftDoorOpenedRotation;

    private Quaternion rightDoorClosedRotation;
    private Quaternion rightDoorOpenedRotation;

    private Coroutine doorCoroutine;

    public string InteractionText
    {
        get
        {
            if (isOpen)
            {
                return openedText;
            }

            return isUnlocked ? unlockedText : lockedText;
        }
    }

    private void Start()
    {
        if (!useDoubleDoor && doorPivot == null)
        {
            doorPivot = transform;
        }

        if (doorPivot != null)
        {
            singleDoorClosedRotation = doorPivot.localRotation;
            singleDoorOpenedRotation = singleDoorClosedRotation * Quaternion.Euler(0f, openAngle, 0f);
        }

        if (leftDoorPivot != null)
        {
            leftDoorClosedRotation = leftDoorPivot.localRotation;
            leftDoorOpenedRotation = leftDoorClosedRotation * Quaternion.Euler(0f, leftDoorOpenAngle, 0f);
        }

        if (rightDoorPivot != null)
        {
            rightDoorClosedRotation = rightDoorPivot.localRotation;
            rightDoorOpenedRotation = rightDoorClosedRotation * Quaternion.Euler(0f, rightDoorOpenAngle, 0f);
        }

        if (interactionCollider != null)
        {
            interactionCollider.enabled = true;
        }
    }

    public void RegisterGemPlaced(GemPedestal pedestal)
    {
        if (pedestal == null)
        {
            return;
        }

        if (placedPedestals.Contains(pedestal))
        {
            return;
        }

        placedPedestals.Add(pedestal);

        Debug.Log("Final door pedestal completed. Current count: " + placedPedestals.Count + "/" + requiredGemCount);

        if (!isUnlocked && placedPedestals.Count >= requiredGemCount)
        {
            UnlockDoor();
        }
    }

    public void Interact()
    {
        if (isOpen)
        {
            return;
        }

        if (!isUnlocked)
        {
            PlayLockedSound();
            Debug.Log("Final door is still sealed.");
            return;
        }

        OpenDoor();
    }

    private void UnlockDoor()
    {
        isUnlocked = true;

        PlayUnlockedSound();

        Debug.Log("Final door unlocked.");
    }

    private void OpenDoor()
    {
        if (isOpen)
        {
            return;
        }

        isOpen = true;

        PlayDoorOpenSound();

        if (doorCoroutine != null)
        {
            StopCoroutine(doorCoroutine);
        }

        doorCoroutine = StartCoroutine(OpenDoorCoroutine());

        Debug.Log("Final door opened.");
    }

    private IEnumerator OpenDoorCoroutine()
    {
        if (useDoubleDoor)
        {
            yield return StartCoroutine(OpenDoubleDoorCoroutine());
        }
        else
        {
            yield return StartCoroutine(OpenSingleDoorCoroutine());
        }

        if (interactionCollider != null)
        {
            interactionCollider.enabled = false;
        }

        yield return new WaitForSeconds(endingDelayAfterDoorOpens);

        StartEnding();
    }

    private IEnumerator OpenSingleDoorCoroutine()
    {
        if (doorPivot == null)
        {
            Debug.LogWarning("Door Pivot is missing on FinalDoorController.");
            yield break;
        }

        while (Quaternion.Angle(doorPivot.localRotation, singleDoorOpenedRotation) > 0.1f)
        {
            doorPivot.localRotation = Quaternion.Lerp(
                doorPivot.localRotation,
                singleDoorOpenedRotation,
                Time.deltaTime * openSpeed
            );

            yield return null;
        }

        doorPivot.localRotation = singleDoorOpenedRotation;
    }

    private IEnumerator OpenDoubleDoorCoroutine()
    {
        if (leftDoorPivot == null || rightDoorPivot == null)
        {
            Debug.LogWarning("Left Door Pivot or Right Door Pivot is missing on FinalDoorController.");
            yield break;
        }

        while (
            Quaternion.Angle(leftDoorPivot.localRotation, leftDoorOpenedRotation) > 0.1f ||
            Quaternion.Angle(rightDoorPivot.localRotation, rightDoorOpenedRotation) > 0.1f
        )
        {
            leftDoorPivot.localRotation = Quaternion.Lerp(
                leftDoorPivot.localRotation,
                leftDoorOpenedRotation,
                Time.deltaTime * openSpeed
            );

            rightDoorPivot.localRotation = Quaternion.Lerp(
                rightDoorPivot.localRotation,
                rightDoorOpenedRotation,
                Time.deltaTime * openSpeed
            );

            yield return null;
        }

        leftDoorPivot.localRotation = leftDoorOpenedRotation;
        rightDoorPivot.localRotation = rightDoorOpenedRotation;
    }

    private void StartEnding()
    {
        if (endingStarted)
        {
            return;
        }

        endingStarted = true;

        if (demoEndingUI != null)
        {
            demoEndingUI.ShowEnding();
        }
        else
        {
            Debug.LogWarning("DemoEndingUI is not assigned on FinalDoorController.");
        }
    }

    private void PlayUnlockedSound()
    {
        if (audioSource != null && unlockedSound != null)
        {
            audioSource.PlayOneShot(unlockedSound);
        }
    }

    private void PlayDoorOpenSound()
    {
        if (audioSource != null && doorOpenSound != null)
        {
            audioSource.PlayOneShot(doorOpenSound);
        }
    }

    private void PlayLockedSound()
    {
        if (audioSource != null && lockedSound != null)
        {
            audioSource.PlayOneShot(lockedSound);
        }
    }
}