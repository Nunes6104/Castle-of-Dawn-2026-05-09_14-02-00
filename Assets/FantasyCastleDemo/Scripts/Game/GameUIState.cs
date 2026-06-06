using UnityEngine;

public class GameUIState : MonoBehaviour
{
    public static GameUIState Instance { get; private set; }

    public bool IsInventoryOpen { get; private set; }
    public bool IsCodePanelOpen { get; private set; }

    public bool IsAnyUIOpen => IsInventoryOpen || IsCodePanelOpen;

    private void Awake()
    {
        Instance = this;
    }

    public void SetInventoryOpen(bool isOpen)
    {
        IsInventoryOpen = isOpen;
    }

    public void SetCodePanelOpen(bool isOpen)
    {
        IsCodePanelOpen = isOpen;
    }
}