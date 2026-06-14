using UnityEngine;

public class GameUIState : MonoBehaviour
{
    public static GameUIState Instance { get; private set; }

    public bool IsInventoryOpen { get; private set; }
    public bool IsCodePanelOpen { get; private set; }
    public bool IsPauseMenuOpen { get; private set; }
    public bool IsEndingOpen { get; private set; }

    public bool IsAnyUIOpen => IsInventoryOpen || IsCodePanelOpen || IsPauseMenuOpen || IsEndingOpen;

    private void Awake()
    {
        Instance = this;
    }

    public void SetInventoryOpen(bool isOpen)
    {
        if (IsEndingOpen)
        {
            IsInventoryOpen = false;
            return;
        }

        IsInventoryOpen = isOpen;
    }

    public void SetCodePanelOpen(bool isOpen)
    {
        if (IsEndingOpen)
        {
            IsCodePanelOpen = false;
            return;
        }

        IsCodePanelOpen = isOpen;
    }

    public void SetPauseMenuOpen(bool isOpen)
    {
        if (IsEndingOpen)
        {
            IsPauseMenuOpen = false;
            return;
        }

        IsPauseMenuOpen = isOpen;
    }

    public void SetEndingOpen(bool isOpen)
    {
        IsEndingOpen = isOpen;

        if (isOpen)
        {
            IsInventoryOpen = false;
            IsCodePanelOpen = false;
            IsPauseMenuOpen = false;
        }
    }
}