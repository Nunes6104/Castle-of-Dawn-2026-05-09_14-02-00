using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenuUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject optionsPanel;

    [Header("Scripts to disable while paused")]
    [SerializeField] private Behaviour[] scriptsToDisable;

    private bool isPaused = false;

    private void Start()
    {
        Time.timeScale = 1f;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (GameUIState.Instance != null)
        {
            GameUIState.Instance.SetPauseMenuOpen(false);
        }
    }

    private void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (GameUIState.Instance != null)
        {
            if (GameUIState.Instance.IsEndingOpen)
            {
                if (isPaused)
                {
                    ForceClosePauseMenu();
                }

                return;
            }

            if (GameUIState.Instance.IsCodePanelOpen)
            {
                return;
            }

            if (GameUIState.Instance.IsInventoryOpen)
            {
                return;
            }
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        if (GameUIState.Instance != null)
        {
            if (GameUIState.Instance.IsEndingOpen ||
                GameUIState.Instance.IsCodePanelOpen ||
                GameUIState.Instance.IsInventoryOpen)
            {
                return;
            }
        }

        isPaused = true;

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }

        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
        }

        if (GameUIState.Instance != null)
        {
            GameUIState.Instance.SetPauseMenuOpen(true);
        }

        SetPlayerScripts(false);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        isPaused = false;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
        }

        if (GameUIState.Instance != null)
        {
            GameUIState.Instance.SetPauseMenuOpen(false);
        }

        Time.timeScale = 1f;
        SetPlayerScripts(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void ForceClosePauseMenu()
    {
        isPaused = false;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
        }

        if (GameUIState.Instance != null)
        {
            GameUIState.Instance.SetPauseMenuOpen(false);
        }

        Time.timeScale = 1f;
    }

    public void OpenOptions()
    {
        if (GameUIState.Instance != null)
        {
            if (GameUIState.Instance.IsEndingOpen ||
                GameUIState.Instance.IsCodePanelOpen ||
                GameUIState.Instance.IsInventoryOpen)
            {
                return;
            }
        }

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        if (optionsPanel != null)
        {
            optionsPanel.SetActive(true);
        }
    }

    public void CloseOptions()
    {
        if (GameUIState.Instance != null)
        {
            if (GameUIState.Instance.IsEndingOpen)
            {
                return;
            }
        }

        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
        }

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }
    }

    public void SetVolume(float value)
    {
        AudioListener.volume = value;
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SetPlayerScripts(true);

        if (GameUIState.Instance != null)
        {
            GameUIState.Instance.SetPauseMenuOpen(false);
            GameUIState.Instance.SetEndingOpen(false);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void SetPlayerScripts(bool enabled)
    {
        foreach (Behaviour script in scriptsToDisable)
        {
            if (script != null)
            {
                script.enabled = enabled;
            }
        }
    }
}