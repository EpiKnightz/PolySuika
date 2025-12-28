using PrimeTween;
using Sortify;
using UnityEngine;

public class UIListButton : UIToggle
{
    [BetterHeader("References")]
    public RectTransform ListButton;
    public RectTransform RestartButton;
    public RectTransform HomeButton;

    [BetterHeader("Listen To")]
    public VoidEventChannelSO ECHomeButtonTriggered;
    public VoidEventChannelSO ECRestartButtonTriggered;
    public IntEventChannelSO ECOnFinalScore;

    // Privates
    private Vector3 DesiredRestartPos;
    private Vector3 DesiredHomePos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        UIStateEnable = false;
        DesiredRestartPos = RestartButton.position - ListButton.position;
        DesiredHomePos = HomeButton.position - ListButton.position;
        RestartButton.position = ListButton.position;
        HomeButton.position = ListButton.position;
    }

    private void OnEnable()
    {
        ECHomeButtonTriggered.Sub(HideList);
        ECRestartButtonTriggered.Sub(HideList);
        ECOnFinalScore.Sub(OnFinalScore);
    }

    private void OnDisable()
    {
        ECHomeButtonTriggered.UnSub(HideList);
        ECRestartButtonTriggered.UnSub(HideList);
        ECOnFinalScore.Unsub(OnFinalScore);
    }

    void HideList()
    {
        if (UIStateEnable)
        {
            OnClick();
        }
    }

    void OnFinalScore(int Score)
    {
        if (!UIStateEnable)
        {
            OnClick();
        }
    }

    public override void EnableVisual()
    {
        // Show buttons
        Tween.Position(RestartButton, ListButton.position + DesiredRestartPos, 0.25f, ease: Ease.InOutSine);
        Tween.Position(HomeButton, ListButton.position + DesiredHomePos, 0.25f, ease: Ease.InOutSine);
    }

    public override void DisableVisual()
    {
        // Hide buttons
        Tween.Position(RestartButton, ListButton.position, 0.25f, ease: Ease.InOutSine);
        Tween.Position(HomeButton, ListButton.position, 0.25f, ease: Ease.InOutSine);
    }
}
