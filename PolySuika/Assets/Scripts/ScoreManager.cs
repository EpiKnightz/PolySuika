using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using Utilities;

public class ScoreManager : MonoBehaviour
{
    
    private int TotalScore = 0;
    private int CurrentScore = 0;
    private int ScoreMultiplier = 0;
    private int Combo = 0;
    public float ComboDuration = 3f;
    private float CurrentComboTime = 0f;

    public event FloatEvent EOnComboTimeChange;
    public event IntEvent EOnScoreMultiChange;
    public event IntEvent EOnCurrentScoreChange;
    public event Int2Event EOnCurrentScoreAndMultiChange;
    public event IntEvent EOnScoreTotalChange;
    public event VoidEvent EOnComboEnd;

    private void Start()
    {
        UIComboSlider comboSlider = FindAnyObjectByType<UIComboSlider>();
        if (comboSlider != null)
        {
            EOnComboTimeChange += comboSlider.OnComboTimeChanges;
        }
        UIComboText text = FindAnyObjectByType<UIComboText>();
        if (text != null)
        {
            EOnCurrentScoreAndMultiChange += text.OnScoreAndMultiChange;
            EOnComboEnd += text.OnComboEnd;
        }
        UIScoreText scoreText = FindAnyObjectByType<UIScoreText>();
        if (scoreText != null)
        {
            EOnScoreTotalChange += scoreText.UpdateTotalScore;
        }
    }

    public void OnMergeEvent(int Tier)
    {
        CurrentScore += (int)Mathf.Pow(2, Tier);
        EOnCurrentScoreChange?.Invoke(CurrentScore);
        RefreshCombo();
    }

    void RefreshCombo()
    {
        if (CurrentComboTime > 0)
        {
            Combo++;
        }
        else
        {
            Combo = 0;
        }
        ScoreMultiplier = 1 + Combo;
        EOnScoreMultiChange?.Invoke(ScoreMultiplier);
        EOnCurrentScoreAndMultiChange?.Invoke(CurrentScore, ScoreMultiplier);
        CurrentComboTime = ComboDuration;
    }

    private void Update()
    {
        if (CurrentComboTime > 0)
        {
            CurrentComboTime -= Time.deltaTime;
            EOnComboTimeChange?.Invoke(CurrentComboTime / ComboDuration);
        } else if (CurrentComboTime != -1)
        {
            CurrentComboTime = -1;
            EOnComboTimeChange?.Invoke(0f);
            OnComboEnd();
        }
    }

    void OnComboEnd()
    {
        TotalScore += CurrentScore * ScoreMultiplier;
        CurrentScore = 0;
        ScoreMultiplier = 0;
        Combo = 0;
        EOnComboEnd?.Invoke();
        EOnScoreTotalChange?.Invoke(TotalScore);        
    }

}
