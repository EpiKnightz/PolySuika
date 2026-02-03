public class ProcessTimerStatistic : ProcessStatistic<float>
{
    //[BetterHeader("Listen To")]
    //public FloatEventChannelSO ECOnTimerUpdated = null;

    protected override void OnStatisticUpdated(float time)
    {
        if (time > 0)
        {
            UpdateStatString(time);
        }
        else
        {
            ClearStatString();
        }
    }

    protected override string FormatedString(float value)
    {
        return value.ToString("F0");
    }
}
