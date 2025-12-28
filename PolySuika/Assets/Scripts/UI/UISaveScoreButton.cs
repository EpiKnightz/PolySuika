using PrimeTween;
using Sortify;
using TMPro;
using UnityEngine;
using Utilities;

[RequireComponent(typeof(RectTransform))]
public class UISaveScoreButton : MonoBehaviour
{
    [BetterHeader("References")]
    [SerializeField] private TMP_InputField UITextInput;
    [SerializeField] private RectTransform ThisRectTransform;

    [BetterHeader("Variables")]
    [SerializeField] private float HighlightScaleChanges = 0.1f;
    [SerializeField] private float HighlightDuration = 1f;
    [SerializeField] private float StartAnimationY = -105f;

    [Header("Listen To")]
    public VoidEventChannelSO ECOnRestartTriggered;
    public IntEventChannelSO ECOnFinalScore;
    //
    private int FinalScore;

    // Events
    private EntryEvent DAddNewLeaderboardEntry;

    private void Start()
    {
        gameObject.SetActive(false);
        UITextInput.gameObject.SetActive(false);        
        var leaderboardManager = FindAnyObjectByType<LeaderboardManager>();
        if (leaderboardManager != null)
        {
            DAddNewLeaderboardEntry += leaderboardManager.OnNewEntryAdded;
        }
    }

    private void Awake()
    {
        ECOnRestartTriggered.Sub(ResetSaveScore);
        ECOnFinalScore.Sub(OnFinalScore);
    }

    private void OnDestroy()
    {
        ECOnRestartTriggered.UnSub(ResetSaveScore);
        ECOnFinalScore.Unsub(OnFinalScore);
    }

    // Cache the final score and hightlight the button
    public void OnFinalScore(int score)
    {
        if (score > 0)
        {
            gameObject.SetActive(true);
            FinalScore = score;
            ThisRectTransform.localScale = Vector3.one * (1 - HighlightScaleChanges);
            Tween.Scale(ThisRectTransform, (1 + HighlightScaleChanges), HighlightDuration, Ease.InOutSine, -1, CycleMode.Yoyo);          
        }
    }

    public void OnSaveScoreClicked()
    {
        Tween.StopAll(ThisRectTransform);
        ThisRectTransform.localScale = Vector3.one;        
        UITextInput.gameObject.SetActive(true);
        var rectTransform = UITextInput.GetComponent<RectTransform>();
        rectTransform.localScale = new Vector3(0.001f, 0.001f, rectTransform.localScale.z);
        rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, StartAnimationY, rectTransform.localPosition.z);
        Tween.Scale(rectTransform, rectTransform.localScale.z, 0.5f, Ease.OutBack).OnComplete(OnTextInputAnimFinished);
        Tween.LocalPositionY(rectTransform, 0, 0.5f, Ease.OutBack);
    }

    void OnTextInputAnimFinished()
    {
        UITextInput.ActivateInputField();
    }

    public void OnNameInput(string name)
    {
        if (!string.IsNullOrEmpty(name))
        {
            ResetSaveScore();
            // Add new Entry
            Entry newEntry = new(name, FinalScore);
            DAddNewLeaderboardEntry?.Invoke(newEntry);
            UIManager.instance.SwitchActivePanel(UIManager.PanelType.Leaderboard, true);
            FinalScore = 0;
        }else
        {
            UITextInput.ActivateInputField();
        }
    }

    void ResetSaveScore()
    {
        UITextInput.text = "";
        UITextInput.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}
