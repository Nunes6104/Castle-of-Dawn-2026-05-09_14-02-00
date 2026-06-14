using System.Collections;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DemoEndingUI : MonoBehaviour
{
    [Header("Ending UI")]
    [SerializeField] private GameObject endingPanel;

    [Header("Player Control")]
    [SerializeField] private GameObject playerObject;
    [SerializeField] private GameObject crosshairObject;

    [Header("Return To Menu")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private float secondsBeforeReturnToMainMenu = 8f;

    private StarterAssetsInputs starterAssetsInputs;
    private FirstPersonController firstPersonController;
    private PlayerInput playerInput;

    private bool endingStarted = false;

    private void Start()
    {
        if (endingPanel != null)
        {
            endingPanel.SetActive(false);
        }

        if (playerObject != null)
        {
            starterAssetsInputs = playerObject.GetComponentInChildren<StarterAssetsInputs>(true);
            firstPersonController = playerObject.GetComponentInChildren<FirstPersonController>(true);
            playerInput = playerObject.GetComponentInChildren<PlayerInput>(true);
        }
    }

    public void ShowEnding()
    {
        if (endingStarted)
        {
            return;
        }

        endingStarted = true;

        Time.timeScale = 1f;

        if (GameUIState.Instance != null)
        {
            GameUIState.Instance.SetEndingOpen(true);
        }

        if (endingPanel != null)
        {
            endingPanel.SetActive(true);
        }

        if (crosshairObject != null)
        {
            crosshairObject.SetActive(false);
        }

        BlockPlayerControl();

        Debug.Log("Demo ending started.");

        StartCoroutine(ReturnToMainMenuAfterDelay());
    }

    private void BlockPlayerControl()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (starterAssetsInputs != null)
        {
            starterAssetsInputs.cursorInputForLook = false;
            starterAssetsInputs.move = Vector2.zero;
            starterAssetsInputs.look = Vector2.zero;
        }

        if (firstPersonController != null)
        {
            firstPersonController.enabled = false;
        }

        if (playerInput != null)
        {
            playerInput.enabled = false;
        }
    }

    private IEnumerator ReturnToMainMenuAfterDelay()
    {
        yield return new WaitForSeconds(secondsBeforeReturnToMainMenu);

        if (GameUIState.Instance != null)
        {
            GameUIState.Instance.SetEndingOpen(false);
        }

        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        SceneManager.LoadScene(mainMenuSceneName);
    }
}