using Unity.VisualScripting;
using UnityEngine;
using Utilities;

public class ScoreManager : MonoBehaviour
{
    private int Combo = 0; // I really should move this to a ScoreManager class
    private int ScoreMultiplier = 0;
    public float ComboDuration = 3f;
    private float currentComboTime = 0f;

    public event FloatEvent EOnComboTimeChange;
    public event IntEvent EOnScoreMultiChange;
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
            EOnScoreMultiChange += text.OnScoreMultiChange;
            EOnComboEnd += text.OnComboEnd;
        }
    }

    public void OnMergeEvent(int Tier)
    {
        RefreshCombo();
    }

    void RefreshCombo()
    {
        if (currentComboTime > 0)
        {
            Combo++;
        }
        else
        {
            Combo = 0;
        }
        ScoreMultiplier = 1 + Combo;
        EOnScoreMultiChange?.Invoke(ScoreMultiplier);
        currentComboTime = ComboDuration;
    }

    private void Update()
    {
        if (currentComboTime > 0)
        {
            currentComboTime -= Time.deltaTime;
            EOnComboTimeChange?.Invoke(currentComboTime / ComboDuration);
        } else if (currentComboTime != -1)
        {
            currentComboTime = -1;
            EOnComboTimeChange?.Invoke(0f);
            EOnComboEnd?.Invoke();
        }
    }


}
