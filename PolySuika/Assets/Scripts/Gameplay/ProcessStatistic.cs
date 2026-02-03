using Sortify;
using UnityEngine;

public class ProcessStatistic<T> : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] protected string StatisticPrefix;

    [BetterHeader("Broadcast On")]
    public StringEventChannelSO ECOnStatStringUpdated = null;
    public IntEventChannelSO ECOnFinalScore = null;

    [BetterHeader("Listen To")]
    public EventChannelSO<T> ECOnStatisticUpdated;

    protected virtual void OnEnable()
    {
        ECOnStatisticUpdated.Sub(OnStatisticUpdated);
    }

    protected virtual void OnDisable()
    {
        ECOnStatisticUpdated.Unsub(OnStatisticUpdated);
    }

    protected virtual void OnStatisticUpdated(T statisticValue)
    {
        UpdateStatString(statisticValue);
    }

    protected virtual int ProcessStatisticValue(T value)
    {
        return 0;
    }

    protected virtual void UpdateStatString(T value)
    {
        ECOnStatStringUpdated?.Invoke(StatisticPrefix + FormatedString(value));
    }

    protected virtual string FormatedString(T value)
    {
        return value.ToString();
    }

    protected void ClearStatString()
    {
        ECOnStatStringUpdated?.Invoke(string.Empty);
    }
}
