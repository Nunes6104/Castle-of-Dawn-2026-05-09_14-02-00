using UnityEngine;

public class DoorSoundEffects : MonoBehaviour
{
    private enum DoorType
    {
        SingleDoor,
        DoubleDoor
    }

    private enum DoorMovementDirection
    {
        None,
        Opening,
        Closing
    }

    [Header("Door Type")]
    [SerializeField] private DoorType doorType = DoorType.SingleDoor;

    [Header("Door References")]
    [SerializeField] private Transform singleDoorTransform;
    [SerializeField] private Transform leftDoorTransform;
    [SerializeField] private Transform rightDoorTransform;

    [Header("Audio Source")]
    [SerializeField] private AudioSource audioSource;

    [Header("Single Door Sounds")]
    [SerializeField] private AudioClip doorOpenSound;
    [SerializeField] private AudioClip doorCloseSound;

    [Header("Double Door Sounds")]
    [SerializeField] private AudioClip doubleDoorOpenSound;
    [SerializeField] private AudioClip doubleDoorCloseSound;

    [Header("Detection Settings")]
    [Tooltip("Minimum rotation change per frame needed to count as movement.")]
    [SerializeField] private float movementThreshold = 0.08f;

    [Tooltip("Turn this on if open and close sounds are reversed.")]
    [SerializeField] private bool invertOpenCloseDetection = false;

    private Quaternion singleClosedRotation;
    private Quaternion leftClosedRotation;
    private Quaternion rightClosedRotation;

    private float previousOpenAmount = 0f;

    private DoorMovementDirection lastSoundDirection = DoorMovementDirection.None;

    private void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (doorType == DoorType.SingleDoor)
        {
            if (singleDoorTransform == null)
            {
                singleDoorTransform = transform;
            }

            singleClosedRotation = singleDoorTransform.localRotation;
            previousOpenAmount = GetSingleDoorOpenAmount();
        }
        else
        {
            if (leftDoorTransform != null)
            {
                leftClosedRotation = leftDoorTransform.localRotation;
            }

            if (rightDoorTransform != null)
            {
                rightClosedRotation = rightDoorTransform.localRotation;
            }

            previousOpenAmount = GetDoubleDoorOpenAmount();
        }
    }

    private void Update()
    {
        float currentOpenAmount = doorType == DoorType.SingleDoor
            ? GetSingleDoorOpenAmount()
            : GetDoubleDoorOpenAmount();

        float movementDifference = currentOpenAmount - previousOpenAmount;

        if (Mathf.Abs(movementDifference) > movementThreshold)
        {
            DoorMovementDirection detectedDirection = movementDifference > 0f
                ? DoorMovementDirection.Opening
                : DoorMovementDirection.Closing;

            if (invertOpenCloseDetection)
            {
                detectedDirection = detectedDirection == DoorMovementDirection.Opening
                    ? DoorMovementDirection.Closing
                    : DoorMovementDirection.Opening;
            }

            if (detectedDirection != lastSoundDirection)
            {
                lastSoundDirection = detectedDirection;

                if (detectedDirection == DoorMovementDirection.Opening)
                {
                    PlayOpenSound();
                }
                else if (detectedDirection == DoorMovementDirection.Closing)
                {
                    PlayCloseSound();
                }
            }
        }

        previousOpenAmount = currentOpenAmount;
    }

    private float GetSingleDoorOpenAmount()
    {
        if (singleDoorTransform == null)
        {
            return 0f;
        }

        return Quaternion.Angle(singleClosedRotation, singleDoorTransform.localRotation);
    }

    private float GetDoubleDoorOpenAmount()
    {
        float totalAmount = 0f;
        int validDoorCount = 0;

        if (leftDoorTransform != null)
        {
            totalAmount += Quaternion.Angle(leftClosedRotation, leftDoorTransform.localRotation);
            validDoorCount++;
        }

        if (rightDoorTransform != null)
        {
            totalAmount += Quaternion.Angle(rightClosedRotation, rightDoorTransform.localRotation);
            validDoorCount++;
        }

        if (validDoorCount == 0)
        {
            return 0f;
        }

        return totalAmount / validDoorCount;
    }

    private void PlayOpenSound()
    {
        if (audioSource == null)
        {
            return;
        }

        AudioClip clipToPlay = doorType == DoorType.SingleDoor
            ? doorOpenSound
            : doubleDoorOpenSound;

        if (clipToPlay != null)
        {
            audioSource.PlayOneShot(clipToPlay);
        }
    }

    private void PlayCloseSound()
    {
        if (audioSource == null)
        {
            return;
        }

        AudioClip clipToPlay = doorType == DoorType.SingleDoor
            ? doorCloseSound
            : doubleDoorCloseSound;

        if (clipToPlay != null)
        {
            audioSource.PlayOneShot(clipToPlay);
        }
    }
}