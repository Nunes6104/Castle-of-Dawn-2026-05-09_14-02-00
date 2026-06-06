using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class CodeChestUI : MonoBehaviour
{
    public static CodeChestUI Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject codePanel;
    [SerializeField] private TMP_InputField codeInputField;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private GameObject crosshair;

    [Header("Player")]
    [SerializeField] private MonoBehaviour playerController;

    private CodeChest currentChest;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        codePanel.SetActive(false);

        if (GameUIState.Instance != null)
        {
            GameUIState.Instance.SetCodePanelOpen(false);
        }

        confirmButton.onClick.AddListener(ConfirmCode);
    }

    private void Update()
    {
        if (codePanel.activeSelf && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ClosePanel();
        }
    }

    public void OpenPanel(CodeChest chest)
    {
        currentChest = chest;

        codePanel.SetActive(true);
        codeInputField.text = "";
        messageText.text = "";

        if (GameUIState.Instance != null)
        {
            GameUIState.Instance.SetCodePanelOpen(true);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (crosshair != null)
        {
            crosshair.SetActive(false);
        }

        if (playerController != null)
        {
            playerController.enabled = false;
        }

        codeInputField.Select();
        codeInputField.ActivateInputField();
    }

    private void ConfirmCode()
    {
        if (currentChest == null)
        {
            return;
        }

        bool correct = currentChest.TryOpenWithCode(codeInputField.text);

        if (correct)
        {
            ClosePanel();
        }
        else
        {
            messageText.text = "Wrong code.";
            codeInputField.text = "";
            codeInputField.Select();
            codeInputField.ActivateInputField();
        }
    }

    private void ClosePanel()
    {
        codePanel.SetActive(false);
        currentChest = null;

        if (GameUIState.Instance != null)
        {
            GameUIState.Instance.SetCodePanelOpen(false);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (crosshair != null)
        {
            crosshair.SetActive(true);
        }

        if (playerController != null)
        {
            playerController.enabled = true;
        }
    }
}