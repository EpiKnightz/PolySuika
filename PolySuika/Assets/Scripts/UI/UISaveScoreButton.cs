using PrimeTween;
using Reflex.Attributes;
using Sortify;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UISaveScoreButton : MonoBehaviour
{
    // Dependencies
    [Inject] private readonly ILeaderboardManager LeaderboardManager;
    [Inject] private readonly IUIManager UIManager;

    [Header("References")]
    [SerializeField] private RectTransform ThisRectTransform;
    [SerializeField] private TMP_InputField UITextInput;
    [SerializeField] private TMP_Text UIFinalScore;

    [BetterHeader("Variables")]
    [SerializeField] private float HighlightScaleChanges = 0.1f;
    [SerializeField] private float HighlightDuration = 1f;
    [SerializeField] private float StartAnimationY = -105f;
    [SerializeField] private float TextInputAnimDuration = 0.5f;

    [BetterHeader("Listen To")]
    public VoidEventChannelSO ECOnRestartTriggered;
    public IntEventChannelSO ECOnFinalScore;
    //
    private int FinalScore;

    private void Awake()
    {
        gameObject.SetActive(false);
        UITextInput.gameObject.SetActive(false);
        OnFinalScore(0);
        ECOnRestartTriggered.Sub(ResetSaveScore);
        ECOnFinalScore.Sub(OnFinalScore);
    }

    private void OnDestroy()
    {
        ECOnRestartTriggered.Unsub(ResetSaveScore);
        ECOnFinalScore.Unsub(OnFinalScore);
    }

    // Cache the final score and hightlight the button
    public void OnFinalScore(int score)
    {
        if (score > 0
            && score != FinalScore
            && LeaderboardManager.CheckLeaderboardEligible(score))
        {
            SaveFinalScore(score);
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
                ThisRectTransform.localScale = Vector3.one * (1 - HighlightScaleChanges);
                Tween.Scale(ThisRectTransform, 1 + HighlightScaleChanges, HighlightDuration, Ease.InOutSine, -1, CycleMode.Yoyo);
            }
            else
            {
                ResetAnim();
            }
        }
    }

    public void OnSaveScoreClicked()
    {
        ResetAnim();
        UITextInput.gameObject.SetActive(true);
        UIFinalScore.gameObject.SetActive(true);
        var rectTransform = UITextInput.GetComponent<RectTransform>();
        rectTransform.localScale = new Vector3(0.001f, 0.001f, rectTransform.localScale.z);
        rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, StartAnimationY, rectTransform.localPosition.z);
        Tween.Scale(rectTransform, rectTransform.localScale.z, TextInputAnimDuration, Ease.OutBack).OnComplete(OnTextInputAnimFinished);
        Tween.LocalPositionY(rectTransform, 0, TextInputAnimDuration, Ease.OutBack);
    }

    private void ResetAnim()
    {
        Tween.StopAll(ThisRectTransform);
        ThisRectTransform.localScale = Vector3.one;
    }

    private void OnTextInputAnimFinished()
    {
        UITextInput.ActivateInputField();
        Tween.Custom(0, FinalScore, TextInputAnimDuration * 3, val =>
        {
            UIFinalScore.text = val.ToString("#,#");
        });
    }

    public void OnNameInput(string name)
    {
        if (!string.IsNullOrEmpty(name))
        {
            // Add new Entry
            Entry newEntry = new(name, FinalScore);
            LeaderboardManager.AddLeaderboardEntry(newEntry);
            UIManager.SwitchActivePanel(PanelType.Leaderboard, true);
            ResetSaveScore();
        }
        else
        {
            UITextInput.ActivateInputField();
        }
    }

    private void ResetSaveScore()
    {
        UITextInput.text = "";
        UITextInput.gameObject.SetActive(false);
        gameObject.SetActive(false);
        SaveFinalScore(0);
    }

    private void SaveFinalScore(int Score)
    {
        FinalScore = Score;
        if (Score == 0)
        {
            UIFinalScore.text = string.Empty;
            UIFinalScore.gameObject.SetActive(false);
        }
    }
}
