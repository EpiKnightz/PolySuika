using PrimeTween;
using Sortify;
using UnityEngine;

internal enum GameState
{
    New,
    Start,
    End,
}

public class CheckTimer : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] private float TimerDuration = 120f;
    [SerializeField] private float UpdateTextInterval = 1f;
    [SerializeField] private float ExplodeInterval = 0.5f;

    [BetterHeader("Broadcast On")]
    public VoidEventChannelSO ECOnGameOver = null;
    public IntFloatEventChannelSO ECOnReachTargetTier = null;
    public FloatEventChannelSO ECOnTimerUpdated = null;

    [BetterHeader("Listen To")]
    public VoidEventChannelSO ECOnGameStart;
    public VoidEventChannelSO ECOnGamePause;
    public VoidEventChannelSO ECOnGameRestart;

    // Privates
    private Tween TimerTween;
    private Tween UpdateTextTween;
    private GameState CurrentState = GameState.New; //To do: maybe move this to another class somewhere

    private void OnEnable()
    {
        TimerTween = new();
        UpdateTextTween = new();
        ECOnGameStart.Sub(StartTimer);
        ECOnGamePause.Sub(PauseTimer);
        ECOnGameRestart.Sub(ResetTimer);
    }

    private void OnDisable()
    {
        ECOnGameStart.Unsub(StartTimer);
        ECOnGamePause.Unsub(PauseTimer);
        ECOnGameRestart.Unsub(ResetTimer);
        ResetTimer();
    }

    private void StartTimer()
    {
        if (!TimerTween.isAlive
            && CurrentState != GameState.End)
        {
            CurrentState = GameState.Start;
            TimerTween = Tween.Delay(TimerDuration, TimerEnd);
            RepeatTweenText();
        }
        else if (TimerTween.isAlive
                && TimerTween.isPaused)
        {
            TimerTween.isPaused = false;
            UpdateTextTween.isPaused = false;
        }
    }

    private void PauseTimer()
    {
        if (TimerTween.isAlive)
        {
            TimerTween.isPaused = true;
            UpdateTextTween.isPaused = true;
            UpdateTweenText();
        }
    }

    private void ResetTimer()
    {
        if (CurrentState == GameState.End
            || (TimerTween.isAlive && !TimerTween.isPaused))
        {
            // Restart case
            TimerTween.Stop();
            UpdateTextTween.Stop();
            CurrentState = GameState.New;
            StartTimer();
        }
        else
        {
            // Clear the game case
            TimerTween.Stop();
            UpdateTextTween.Stop();
            CurrentState = GameState.New;
            ECOnTimerUpdated.Invoke(0);
        }
    }

    private void TimerEnd()
    {
        CurrentState = GameState.End;
        ECOnGameOver.Invoke();
        ECOnReachTargetTier.Invoke(-1, ExplodeInterval);
    }

    private void UpdateTweenText()
    {
        if (TimerTween.isAlive)
        {
            ECOnTimerUpdated.Invoke(TimerTween.duration - TimerTween.elapsedTime);
        }
    }

    private void RepeatTweenText()
    {
        UpdateTweenText();
        if (TimerTween.isAlive
            && TimerTween.duration - TimerTween.elapsedTime >= 1)
        {
            UpdateTextTween = Tween.Delay(UpdateTextInterval, RepeatTweenText);
        }
    }
}
