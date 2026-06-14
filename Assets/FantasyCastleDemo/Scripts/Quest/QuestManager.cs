using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    private enum PuzzleQuestState
    {
        Explore,
        FindCode,
        OpenInventory,
        OpenChest,
        SolvePuzzle,
        CollectReward,
        Completed
    }

    [Header("Quest UI")]
    [SerializeField] private GameObject questPanel;
    [SerializeField] private TMP_Text questText;
    [SerializeField] private CanvasGroup questCanvasGroup;

    [Header("Fade Settings")]
    [SerializeField] private bool useFade = true;
    [SerializeField] private float fadeDuration = 0.25f;

    [Header("Barracks / Code Puzzle Text")]
    [SerializeField] private string barracksExploreText = "Explore the barracks";
    [SerializeField] private string barracksFindCodeText = "Find the code";
    [SerializeField] private string barracksOpenInventoryText = "Press TAB to check your inventory";
    [SerializeField] private string barracksOpenChestText = "Open the chest with the code";
    [SerializeField] private string barracksCollectRewardText = "Collect the Strange Gem";

    [Header("Smithy / Lever Puzzle Text")]
    [SerializeField] private string smithyExploreText = "Explore the smithy";
    [SerializeField] private string smithySolveText = "Solve the lever puzzle";
    [SerializeField] private string smithyCollectRewardText = "Collect the Strange Star";

    [Header("Throne Room / Statue Puzzle Text")]
    [SerializeField] private string throneRoomExploreText = "Explore the throne room";
    [SerializeField] private string throneRoomSolveText = "Solve the statue riddle";
    [SerializeField] private string throneRoomCollectRewardText = "Collect the Strange Ruby";

    [Header("Final Objective Text")]
    [SerializeField] private string findExitText = "Find the exit";

    [Header("Ending")]
    [SerializeField] private bool hideQuestUIOnEnding = true;

    private PuzzleQuestState barracksState = PuzzleQuestState.Explore;
    private PuzzleQuestState smithyState = PuzzleQuestState.Explore;
    private PuzzleQuestState throneRoomState = PuzzleQuestState.Explore;

    private bool finalObjectiveUnlocked = false;
    private bool endingStarted = false;

    private Coroutine refreshCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        if (questCanvasGroup == null && questPanel != null)
        {
            questCanvasGroup = questPanel.GetComponent<CanvasGroup>();

            if (questCanvasGroup == null)
            {
                questCanvasGroup = questPanel.AddComponent<CanvasGroup>();
            }
        }

        if (questPanel != null)
        {
            questPanel.SetActive(true);
        }

        if (questCanvasGroup != null)
        {
            questCanvasGroup.alpha = 1f;
        }

        RefreshQuestUIInstant();
    }

    public void OnCodeChestInteractedWithoutCode()
    {
        if (barracksState == PuzzleQuestState.Explore)
        {
            barracksState = PuzzleQuestState.FindCode;
            RefreshQuestUI();
        }
    }

    public void OnCodePaperPickedUp()
    {
        if (barracksState == PuzzleQuestState.Explore || barracksState == PuzzleQuestState.FindCode)
        {
            barracksState = PuzzleQuestState.OpenInventory;
            RefreshQuestUI();
        }
    }

    public void OnInventoryOpened()
    {
        if (barracksState == PuzzleQuestState.OpenInventory)
        {
            barracksState = PuzzleQuestState.OpenChest;
            RefreshQuestUI();
        }
    }

    public void OnCodeChestOpened()
    {
        if (barracksState != PuzzleQuestState.Completed)
        {
            barracksState = PuzzleQuestState.CollectReward;
            RefreshQuestUI();
        }
    }

    public void OnBarracksRewardCollected()
    {
        if (barracksState != PuzzleQuestState.Completed)
        {
            barracksState = PuzzleQuestState.Completed;
            CheckIfAllPuzzlesCompleted();
            RefreshQuestUI();
        }
    }

    public void OnSmithyEntered()
    {
        if (smithyState == PuzzleQuestState.Explore)
        {
            smithyState = PuzzleQuestState.SolvePuzzle;
            RefreshQuestUI();
        }
    }

    public void OnLeverChestOpened()
    {
        if (smithyState != PuzzleQuestState.Completed)
        {
            smithyState = PuzzleQuestState.CollectReward;
            RefreshQuestUI();
        }
    }

    public void OnSmithyRewardCollected()
    {
        if (smithyState != PuzzleQuestState.Completed)
        {
            smithyState = PuzzleQuestState.Completed;
            CheckIfAllPuzzlesCompleted();
            RefreshQuestUI();
        }
    }

    public void OnThroneRoomEntered()
    {
        if (throneRoomState == PuzzleQuestState.Explore)
        {
            throneRoomState = PuzzleQuestState.SolvePuzzle;
            RefreshQuestUI();
        }
    }

    public void OnStatueChestOpened()
    {
        if (throneRoomState != PuzzleQuestState.Completed)
        {
            throneRoomState = PuzzleQuestState.CollectReward;
            RefreshQuestUI();
        }
    }

    public void OnThroneRoomRewardCollected()
    {
        if (throneRoomState != PuzzleQuestState.Completed)
        {
            throneRoomState = PuzzleQuestState.Completed;
            CheckIfAllPuzzlesCompleted();
            RefreshQuestUI();
        }
    }

    public void OnFinalDoorOpened()
    {
        endingStarted = true;

        if (hideQuestUIOnEnding)
        {
            HideQuestUI();
        }
        else
        {
            RefreshQuestUI();
        }
    }

    public void HideQuestUI()
    {
        if (questPanel != null)
        {
            questPanel.SetActive(false);
        }
    }

    public void ShowQuestUI()
    {
        if (questPanel != null)
        {
            questPanel.SetActive(true);
        }

        RefreshQuestUI();
    }

    private void CheckIfAllPuzzlesCompleted()
    {
        if (barracksState == PuzzleQuestState.Completed &&
            smithyState == PuzzleQuestState.Completed &&
            throneRoomState == PuzzleQuestState.Completed)
        {
            finalObjectiveUnlocked = true;
        }
    }

    private void RefreshQuestUI()
    {
        if (refreshCoroutine != null)
        {
            StopCoroutine(refreshCoroutine);
        }

        if (useFade && questCanvasGroup != null)
        {
            refreshCoroutine = StartCoroutine(RefreshQuestUIWithFade());
        }
        else
        {
            RefreshQuestUIInstant();
        }
    }

    private IEnumerator RefreshQuestUIWithFade()
    {
        yield return FadeQuestUI(1f, 0f);

        RefreshQuestUIInstant();

        yield return FadeQuestUI(0f, 1f);

        refreshCoroutine = null;
    }

    private IEnumerator FadeQuestUI(float startAlpha, float targetAlpha)
    {
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;

            if (questCanvasGroup != null)
            {
                questCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / fadeDuration);
            }

            yield return null;
        }

        if (questCanvasGroup != null)
        {
            questCanvasGroup.alpha = targetAlpha;
        }
    }

    private void RefreshQuestUIInstant()
    {
        if (questText == null)
        {
            return;
        }

        questText.text = BuildQuestText();
    }

    private string BuildQuestText()
    {
        if (endingStarted)
        {
            return "";
        }

        List<string> activeObjectives = new List<string>();

        if (finalObjectiveUnlocked)
        {
            activeObjectives.Add(GetUncheckedLine(findExitText));
            return string.Join("\n", activeObjectives);
        }

        string barracksObjective = GetBarracksObjective();
        string smithyObjective = GetSmithyObjective();
        string throneRoomObjective = GetThroneRoomObjective();

        if (!string.IsNullOrEmpty(barracksObjective))
        {
            activeObjectives.Add(GetUncheckedLine(barracksObjective));
        }

        if (!string.IsNullOrEmpty(smithyObjective))
        {
            activeObjectives.Add(GetUncheckedLine(smithyObjective));
        }

        if (!string.IsNullOrEmpty(throneRoomObjective))
        {
            activeObjectives.Add(GetUncheckedLine(throneRoomObjective));
        }

        return string.Join("\n", activeObjectives);
    }

    private string GetBarracksObjective()
    {
        switch (barracksState)
        {
            case PuzzleQuestState.Explore:
                return barracksExploreText;

            case PuzzleQuestState.FindCode:
                return barracksFindCodeText;

            case PuzzleQuestState.OpenInventory:
                return barracksOpenInventoryText;

            case PuzzleQuestState.OpenChest:
                return barracksOpenChestText;

            case PuzzleQuestState.CollectReward:
                return barracksCollectRewardText;

            case PuzzleQuestState.Completed:
                return "";

            default:
                return barracksExploreText;
        }
    }

    private string GetSmithyObjective()
    {
        switch (smithyState)
        {
            case PuzzleQuestState.Explore:
                return smithyExploreText;

            case PuzzleQuestState.SolvePuzzle:
                return smithySolveText;

            case PuzzleQuestState.CollectReward:
                return smithyCollectRewardText;

            case PuzzleQuestState.Completed:
                return "";

            default:
                return smithyExploreText;
        }
    }

    private string GetThroneRoomObjective()
    {
        switch (throneRoomState)
        {
            case PuzzleQuestState.Explore:
                return throneRoomExploreText;

            case PuzzleQuestState.SolvePuzzle:
                return throneRoomSolveText;

            case PuzzleQuestState.CollectReward:
                return throneRoomCollectRewardText;

            case PuzzleQuestState.Completed:
                return "";

            default:
                return throneRoomExploreText;
        }
    }

    private string GetUncheckedLine(string text)
    {
        return "[ ] " + text;
    }
}