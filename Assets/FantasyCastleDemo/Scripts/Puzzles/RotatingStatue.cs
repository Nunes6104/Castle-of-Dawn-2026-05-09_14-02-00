using System.Collections;
using UnityEngine;

public class RotatingStatue : MonoBehaviour, IInteractable
{
    [Header("Statue Settings")]
    [SerializeField] private string statueName = "Seer";

    [Tooltip("Correct Y rotation needed to solve the puzzle.")]
    [SerializeField] private float correctYRotation = 90f;

    [Tooltip("How many degrees the statue rotates per interaction.")]
    [SerializeField] private float rotationStep = 45f;

    [Tooltip("How fast the statue rotates visually.")]
    [SerializeField] private float rotationSpeed = 180f;

    [Header("Reset Settings")]
    [SerializeField] private float resetYRotation = 0f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip rotateSound;

    [Tooltip("How long the statue rotation sound should last.")]
    [SerializeField] private float rotateSoundDuration = 1f;

    [Header("Interaction Text")]
    [SerializeField] private string interactionText = "Press E to rotate statue";
    [SerializeField] private string rotatingText = "Statue is rotating";

    private bool isRotating = false;
    private Coroutine rotationCoroutine;
    private Coroutine soundCoroutine;

    public string InteractionText => isRotating ? rotatingText : interactionText;

    public string StatueName => statueName;
    public float CorrectYRotation => NormalizeAngle(correctYRotation);

    public void Interact()
    {
        if (isRotating)
        {
            return;
        }

        RotateStatue();
    }

    private void RotateStatue()
    {
        Vector3 currentRotation = transform.localEulerAngles;

        float currentYRotation = NormalizeAngle(currentRotation.y);
        float targetYRotation = NormalizeAngle(currentYRotation + rotationStep);

        RotateToY(targetYRotation, true);
    }

    public void ResetStatue()
    {
        RotateToY(resetYRotation, true);
    }

    private void RotateToY(float targetYRotation, bool playSound)
    {
        if (rotationCoroutine != null)
        {
            StopCoroutine(rotationCoroutine);
        }

        rotationCoroutine = StartCoroutine(RotateStatueCoroutine(NormalizeAngle(targetYRotation), playSound));
    }

    private IEnumerator RotateStatueCoroutine(float targetYRotation, bool playSound)
    {
        isRotating = true;

        if (playSound)
        {
            PlayRotateSound();
        }

        Quaternion targetRotation = Quaternion.Euler(
            transform.localEulerAngles.x,
            targetYRotation,
            transform.localEulerAngles.z
        );

        while (Quaternion.Angle(transform.localRotation, targetRotation) > 0.1f)
        {
            transform.localRotation = Quaternion.RotateTowards(
                transform.localRotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

            yield return null;
        }

        transform.localRotation = targetRotation;

        Debug.Log(statueName + " rotated to Y: " + NormalizeAngle(transform.localEulerAngles.y));

        isRotating = false;
    }

    private void PlayRotateSound()
    {
        if (audioSource == null || rotateSound == null)
        {
            return;
        }

        if (soundCoroutine != null)
        {
            StopCoroutine(soundCoroutine);
        }

        audioSource.Stop();
        audioSource.clip = rotateSound;
        audioSource.loop = false;

        if (rotateSoundDuration > 0f)
        {
            audioSource.pitch = rotateSound.length / rotateSoundDuration;
        }
        else
        {
            audioSource.pitch = 1f;
        }

        audioSource.Play();

        soundCoroutine = StartCoroutine(StopRotateSoundAfterDuration());
    }

    private IEnumerator StopRotateSoundAfterDuration()
    {
        yield return new WaitForSeconds(rotateSoundDuration);

        if (audioSource != null)
        {
            audioSource.Stop();
            audioSource.pitch = 1f;
        }
    }

    public bool IsCorrectlyRotated()
    {
        float currentYRotation = NormalizeAngle(transform.localEulerAngles.y);
        float targetYRotation = NormalizeAngle(correctYRotation);

        return Mathf.Approximately(currentYRotation, targetYRotation);
    }

    private float NormalizeAngle(float angle)
    {
        angle %= 360f;

        if (angle < 0f)
        {
            angle += 360f;
        }

        return Mathf.Round(angle);
    }
}