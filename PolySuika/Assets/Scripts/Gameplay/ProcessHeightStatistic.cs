using UnityEngine;

public class ProcessHeightStatistic : ProcessStatistic<float>
{
    [Header("Variables")]
    [SerializeField] private int StatOffset = 0;
    [SerializeField] private int MinimumLeaderboardScore = 200;

    // Privates
    private int OldScore = -1;

    protected override void OnEnable()
    {
        base.OnEnable();
        OldScore = -1;
    }

    protected override void OnStatisticUpdated(float statisticValue)
    {
        if (statisticValue == 0)
        {
            ClearStatString();
            OldScore = -1;
            return;
        }
        int resultValue = ProcessStatisticValue(statisticValue);
        if (resultValue != OldScore)
        {
            UpdateStatString(resultValue);
            OldScore = resultValue;
            if (resultValue > MinimumLeaderboardScore)
            {
                ECOnFinalScore?.Invoke(resultValue);
            }
        }
    }

    protected override int ProcessStatisticValue(float value)
    {
        return StatOffset + Mathf.FloorToInt(100f * value);
    }

    protected override string FormatedString(float value)
    {
        return value.ToString("#,#");
    }
}
